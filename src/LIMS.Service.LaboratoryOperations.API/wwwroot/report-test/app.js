const authority = "http://localhost:8081/realms/lims";
const clientId = "lims-frontend";
const redirectUri = `${location.origin}${location.pathname}`;
const tokenKey = "lims-report-test.tokens";
const stateKey = "lims-report-test.oauth-state";
const verifierKey = "lims-report-test.pkce-verifier";
const guidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-8][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

const elements = {
  login: document.getElementById("login-button"),
  connect: document.getElementById("connect-button"),
  logout: document.getElementById("logout-button"),
  connection: document.getElementById("connection-status"),
  user: document.getElementById("user-info"),
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
let refreshInProgress;
let requestInProgress = false;
const operationKey = "lims-report-test.operation";

function storedTokens() {
  try {
    return JSON.parse(sessionStorage.getItem(tokenKey) || "null");
  } catch {
    return null;
  }
}

function storeTokens(response) {
  const previous = storedTokens();
  sessionStorage.setItem(tokenKey, JSON.stringify({
    accessToken: response.access_token,
    refreshToken: response.refresh_token || previous?.refreshToken,
    expiresAt: Date.now() + response.expires_in * 1000
  }));
  updateControls();
}

function tokenClaims(accessToken) {
  try {
    const payload = accessToken.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
    const bytes = Uint8Array.from(atob(payload), character => character.charCodeAt(0));
    return JSON.parse(new TextDecoder().decode(bytes));
  } catch {
    return {};
  }
}

function updateControls() {
  const tokens = storedTokens();
  const claims = tokens ? tokenClaims(tokens.accessToken) : {};
  elements.login.disabled = Boolean(tokens);
  elements.connect.disabled = !tokens || Boolean(polling) || !sessionStorage.getItem(operationKey);
  elements.logout.disabled = !tokens;
  elements.loadOrders.disabled = !tokens;
  elements.create.disabled = !tokens || Boolean(polling) || requestInProgress;
  elements.user.textContent = tokens
    ? `Пользователь: ${claims.preferred_username || "—"} · user_id: ${claims.user_id || "отсутствует в access token"}`
    : "Войдите через Keycloak, чтобы получить токен пользователя.";
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

function randomBase64Url(length) {
  const bytes = crypto.getRandomValues(new Uint8Array(length));
  return btoa(String.fromCharCode(...bytes)).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

async function sha256Base64Url(value) {
  const digest = await crypto.subtle.digest("SHA-256", new TextEncoder().encode(value));
  return btoa(String.fromCharCode(...new Uint8Array(digest)))
    .replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

async function beginLogin() {
  const state = randomBase64Url(24);
  const verifier = randomBase64Url(32);
  sessionStorage.setItem(stateKey, state);
  sessionStorage.setItem(verifierKey, verifier);

  const url = new URL(`${authority}/protocol/openid-connect/auth`);
  url.search = new URLSearchParams({
    client_id: clientId,
    redirect_uri: redirectUri,
    response_type: "code",
    scope: "openid",
    state,
    code_challenge: await sha256Base64Url(verifier),
    code_challenge_method: "S256"
  }).toString();
  location.assign(url.href);
}

async function requestTokens(parameters) {
  const response = await fetch(`${authority}/protocol/openid-connect/token`, {
    method: "POST",
    headers: { "Content-Type": "application/x-www-form-urlencoded" },
    body: new URLSearchParams(parameters)
  });
  if (!response.ok) {
    throw new Error(`Keycloak вернул HTTP ${response.status} при получении токена.`);
  }
  return response.json();
}

async function finishLogin() {
  const query = new URLSearchParams(location.search);
  if (!query.has("code") && !query.has("error")) return;

  history.replaceState({}, "", redirectUri);
  const expectedState = sessionStorage.getItem(stateKey);
  const verifier = sessionStorage.getItem(verifierKey);
  sessionStorage.removeItem(stateKey);
  sessionStorage.removeItem(verifierKey);

  if (query.has("error")) throw new Error(`Keycloak: ${query.get("error_description") || query.get("error")}`);
  if (!expectedState || !verifier || query.get("state") !== expectedState) {
    throw new Error("Не совпало состояние входа. Запустите авторизацию заново.");
  }

  const tokens = await requestTokens({
    grant_type: "authorization_code",
    client_id: clientId,
    code: query.get("code"),
    redirect_uri: redirectUri,
    code_verifier: verifier
  });
  storeTokens(tokens);
  addEvent("Вход выполнен");
}

async function accessToken() {
  const tokens = storedTokens();
  if (!tokens) throw new Error("Сначала войдите через Keycloak.");
  if (Date.now() < tokens.expiresAt - 30_000) return tokens.accessToken;
  if (!tokens.refreshToken) throw new Error("Срок действия токена истёк. Войдите снова.");

  if (!refreshInProgress) {
    refreshInProgress = requestTokens({
      grant_type: "refresh_token",
      client_id: clientId,
      refresh_token: tokens.refreshToken
    }).then(storeTokens).finally(() => { refreshInProgress = undefined; });
  }
  await refreshInProgress;
  return storedTokens().accessToken;
}

async function authorizedFetch(path, options = {}) {
  const response = await fetch(path, {
    ...options,
    headers: { ...options.headers, Authorization: `Bearer ${await accessToken()}` }
  });
  if (!response.ok) {
    const body = await response.text();
    throw new Error(`HTTP ${response.status}${body ? `: ${body.slice(0, 300)}` : ""}`);
  }
  return response;
}

async function loadOrders() {
  const response = await authorizedFetch("/api/orders");
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
    const response = await authorizedFetch(`/api/orders/${orderId}/report-async`, { method: "POST" });
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

elements.login.addEventListener("click", run(beginLogin));
elements.connect.addEventListener("click", run(waitForOperation));
elements.loadOrders.addEventListener("click", run(loadOrders));
elements.create.addEventListener("click", run(createOperation));
elements.orders.addEventListener("change", () => { elements.orderId.value = elements.orders.value; });
elements.clear.addEventListener("click", () => {
  elements.events.replaceChildren();
  elements.empty.hidden = false;
});
elements.logout.addEventListener("click", run(async () => {
  polling?.abort();
  polling = undefined;
  sessionStorage.removeItem(tokenKey);
  sessionStorage.removeItem(operationKey);
  setConnectionStatus("Нет активного ожидания");
  setResult("Локальная сессия очищена.");
}));

run(async () => {
  await finishLogin();
  updateControls();
  if (storedTokens() && sessionStorage.getItem(operationKey)) await waitForOperation();
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
          headers: { Authorization: `Bearer ${await accessToken()}` },
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
