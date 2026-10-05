const guidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-8][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

const elements = {
  connect: document.getElementById("connect-button"),
  connection: document.getElementById("connection-status"),
  orderId: document.getElementById("order-id"),
  loadOrders: document.getElementById("load-orders-button"),
  orders: document.getElementById("orders-select"),
  create: document.getElementById("create-button"),
  result: document.getElementById("request-result"),
  events: document.getElementById("events-list"),
  empty: document.getElementById("empty-events"),
  clear: document.getElementById("clear-button")
};

let polling;
let requestInProgress = false;
const operationKey = "lims-report-test.operation";

function updateControls() {
  elements.connect.disabled = Boolean(polling) || !sessionStorage.getItem(operationKey);
  elements.create.disabled = Boolean(polling) || requestInProgress;
}
function setConnectionStatus(text, className = "") {
  elements.connection.textContent = text;
  elements.connection.className = `badge ${className}`.trim();
  updateControls();
}

function setResult(text, kind = "") {
  elements.result.textContent = text;
  elements.result.className = `result ${kind || "muted"}`;
}

function addEvent(title, data) {
  elements.empty.hidden = true;
  const item = document.createElement("li");
  const time = document.createElement("time");
  time.textContent = new Date().toLocaleTimeString("ru-RU");
  const heading = document.createElement("strong");
  heading.textContent = title;
  item.append(time, heading);
  if (data !== undefined) {
    const details = document.createElement("pre");
    details.textContent = JSON.stringify(data, null, 2);
    item.append(details);
  }
  elements.events.prepend(item);
}

async function apiFetch(path, options = {}) {
  const response = await fetch(path, options);
  if (!response.ok) {
    const body = await response.text();
    throw new Error(`HTTP ${response.status}${body ? `: ${body.slice(0, 300)}` : ""}`);
  }
  return response;
}

async function loadOrders() {
  const response = await apiFetch("/api/orders");
  const orders = await response.json();
  elements.orders.replaceChildren();
  for (const order of orders) {
    const option = document.createElement("option");
    option.value = order.id;
    option.textContent = `${order.name || order.code || "Заказ"} · ${order.id}`;
    elements.orders.append(option);
  }
  elements.orders.hidden = orders.length === 0;
  if (orders.length) elements.orderId.value = orders[0].id;
  addEvent("Заказы загружены", { count: orders.length });
}

async function createOperation() {
  const orderId = elements.orderId.value.trim();
  if (!guidPattern.test(orderId)) throw new Error("Введите корректный GUID заказа.");

  requestInProgress = true;
  updateControls();
  setResult("Отправляем запрос…");
  try {
    const response = await apiFetch(`/api/orders/${orderId}/report-async`, { method: "POST" });
    const operation = await response.json();
    sessionStorage.setItem(operationKey, operation.operationId);
    setResult(`Операция ${operation.operationId} создана. Ожидаем завершения…`);
    addEvent("Операция создана", { orderId, ...operation });
    void run(waitForOperation)();
  } finally {
    requestInProgress = false;
    updateControls();
  }
}

function run(action) {
  return async () => {
    try {
      await action();
    } catch (error) {
      setResult(error.message, "error");
      addEvent("Ошибка", { message: error.message });
    }
  };
}

elements.connect.addEventListener("click", run(waitForOperation));
elements.loadOrders.addEventListener("click", run(loadOrders));
elements.create.addEventListener("click", run(createOperation));
elements.orders.addEventListener("change", () => { elements.orderId.value = elements.orders.value; });
elements.clear.addEventListener("click", () => {
  elements.events.replaceChildren();
  elements.empty.hidden = false;
});

run(async () => {
  updateControls();
  if (sessionStorage.getItem(operationKey)) await waitForOperation();
})();

async function waitForOperation() {
  if (polling) return;
  const id = sessionStorage.getItem(operationKey);
  if (!id) return;
  const controller = new AbortController();
  polling = controller;
  setConnectionStatus("Ожидание HTTP…", "connecting");
  let failures = 0;
  try {
    while (!controller.signal.aborted) {
      let response;
      try {
        response = await fetch(`/api/background-operations/${encodeURIComponent(id)}/wait`, {
          cache: "no-store",
          signal: controller.signal
        });
        if (response.status >= 500) throw new Error(`HTTP ${response.status}`);
        failures = 0;
      } catch (error) {
        if (controller.signal.aborted) return;
        if (++failures > 3) throw error;
        addEvent("Повтор запроса ожидания", { attempt: failures, error: error.message });
        await new Promise(resolve => {
          const finish = () => {
            clearTimeout(timer);
            controller.signal.removeEventListener("abort", finish);
            resolve();
          };
          const timer = setTimeout(finish, failures * 2000);
          controller.signal.addEventListener("abort", finish, { once: true });
        });
        continue;
      }
      if (response.status === 204) continue;
      if (!response.ok) {
        if (response.status === 404) sessionStorage.removeItem(operationKey);
        throw new Error(`Ожидание операции: HTTP ${response.status}. Проверьте сессию и доступ к операции.`);
      }
      const operation = await response.json();
      if (controller.signal.aborted) return;
      addEvent("Операция завершена", operation);
      sessionStorage.removeItem(operationKey);
      setConnectionStatus("Завершено", "connected");
      setResult(operation.status === "Succeeded"
        ? `Отчёт готов. Операция: ${id}`
        : `Операция ${id}: ${operation.status}. ${operation.error || ""}`,
      operation.status === "Succeeded" ? "success" : "error");
      return;
    }
  } catch (error) {
    if (!controller.signal.aborted) {
      setConnectionStatus("Ожидание прервано");
      throw error;
    }
  } finally {
    if (polling === controller) {
      polling = undefined;
      updateControls();
    }
  }
}
