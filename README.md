# LIMS.DDD

<img width="2883" height="1578" alt="image" src="https://github.com/user-attachments/assets/64d4e4af-2bbe-438f-956f-a27acbd0b689" />

Учебный проект лабораторной информационной системы (LIMS), построенный вокруг **Domain-Driven Design**, **Clean Architecture** и микросервисного подхода.

Проект моделирует жизненный цикл лабораторных операций: от регистрации задания и проб до проведения исследований, ввода измерений и автоматического расчёта результатов.

> Проект находится в активной разработке и в первую очередь предназначен для изучения DDD, архитектурных паттернов, взаимодействия микросервисов и построения бизнес-ориентированной модели.

---

## Содержание

* [Архитектура](#архитектура)
* [Структура проекта](#структура-проекта)
* [Запуск проекта](#запуск-проекта)

  * [Требования](#требования)
  * [Запуск RabbitMQ](#запуск-rabbitmq)
  * [Настройка базы данных](#настройка-базы-данных)
  * [Применение миграций](#применение-миграций)
  * [Запуск сервисов](#запуск-сервисов)
  * [Сервисы проекта](#сервисы-проекта)
* [Жизненные циклы](#жизненные-циклы)

  * [Методика](#жизненный-цикл-методики)
  * [Задание](#жизненный-цикл-задания-order)
  * [Проба](#жизненный-цикл-пробы-sample)
  * [Исследование](#жизненный-цикл-исследования-study)
* [Оркестрация лабораторного процесса](#оркестрация-лабораторного-процесса)
* [Бизнес-процессы](#бизнес-процессы)

  * [Сценарий создания](#сценарий-создания)
  * [Конфигурирование методики](#конфигурирование-методики)
  * [Частичное обновление структуры](#частичное-обновление-структуры)
  * [Контроль изменений](#контроль-изменений)
  * [Движок расчётов](#движок-расчётов)
* [Использованные архитектурные паттерны](#использованные-архитектурные-паттерны)
* [Архитектурные принципы](#архитектурные-принципы)
* [Технологии](#технологии)
* [Нормативная основа](#нормативная-основа)
* [Цель проекта](#цель-проекта)

---

# Архитектура

Проект разделён на несколько микросервисов и общих библиотек.

На текущий момент в репозитории присутствуют:

* **Guides.Service** — сервис для хранения статических данных, например Ед. изм.
* **LIMS.Service.Methodologies** — управление методиками, параметрами, результатами и правилами расчёта.
* **LIMS.Service.LaboratoryOperations** — управление заданиями, пробами и исследованиями.
* **RabbitMq.Library.Broker** — библиотека для работы с RabbitMQ.
* **RabbitMq.Library.Outbox** — переиспользуемая реализация Outbox Pattern.
* **Broker.Messages** — контракты интеграционных сообщений.
* `Application.SeedWork` и `Domain.SeedWork` — общие примитивы Application и Domain слоёв.

Структура `src` отражает разделение сервисов на отдельные Application, Domain, Infrastructure, Persistence и API проекты.

Каждый бизнес-контекст имеет собственный `DbContext` и собственный набор persistence-компонентов. Например, `LIMS.Service.Methodologies.Persistence` содержит `ApplicationDbContext`, репозитории, Unit of Work и EF Core migrations.

---

# Структура проекта

Каждый основной микросервис разделён по слоям:

```text
LIMS.Service.Methodologies
│
├── API
├── Application
├── Domain
├── Infrastructure
└── Persistence
```

### API

Отвечает за транспортный слой:

* HTTP endpoints;
* HTTP request/response;
* DI composition;
* Swagger/OpenAPI.

### Application

Содержит application use cases:

* Commands;
* Queries;
* Handlers;
* orchestration;
* взаимодействие с репозиториями;
* Unit of Work.

Application Layer не содержит инфраструктурной реализации RabbitMQ, EF Core или других внешних механизмов.

### Domain

Содержит бизнес-модель:

* Aggregates;
* Entities;
* Value Objects;
* Domain Services;
* Domain Rules;
* Domain Errors.

Основная задача Domain Layer — поддерживать бизнес-инварианты независимо от способа доставки HTTP-запроса или хранения данных.

### Persistence

Отвечает за:

* EF Core;
* `DbContext`;
* PostgreSQL;
* Repository implementations;
* Unit of Work;
* database configurations;
* migrations.

Например, `LIMS.Service.Methodologies.Persistence` содержит отдельные `Repositories`, `Configurations`, `Migrations` и `ApplicationDbContext`.

### Infrastructure

Содержит интеграции с внешними системами:

* RabbitMQ;
* integration events;
* message handlers;
* Outbox;
* другие инфраструктурные зависимости.

---

# Запуск проекта

## Требования

Для запуска проекта необходимо установить:

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* [Docker](https://www.docker.com/)
* PostgreSQL
* Git

---

## 1. Клонирование репозитория

```bash
git clone https://github.com/SaintEfim/LIMS.DDD.git
cd LIMS.DDD
```

---

## 2. Запуск RabbitMQ

Для работы интеграционных событий необходимо запустить RabbitMQ.

Используется официальный Docker image с management UI:

```bash
docker run -it --rm \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

После запуска:

* AMQP: `localhost:5672`
* RabbitMQ Management UI: [http://localhost:15672](http://localhost:15672)

Для стандартной конфигурации контейнера:

```text
Username: guest
Password: guest
```

RabbitMQ используется сервисами для асинхронного обмена интеграционными событиями.

---

## 3. Настройка PostgreSQL

Каждый бизнес-сервис имеет собственный `DbContext` и собственную persistence-конфигурацию.

```text
LIMS.Service.Methodologies
        ↓
ApplicationDbContext
        ↓
PostgreSQL

LIMS.Service.LaboratoryOperations
        ↓
ApplicationDbContext
        ↓
PostgreSQL
```

Connection string должен соответствовать настройкам локального PostgreSQL.

---

## 4. Применение миграций

Перед первым запуском необходимо применить EF Core migrations.

Для каждого сервиса необходимо применить соответствующие миграции:

```bash
dotnet ef database update \
  --project src/LIMS.Service.Methodologies.Persistence \
  --startup-project src/LIMS.Service.Methodologies.API

dotnet ef database update \
  --project src/LIMS.Service.LaboratoryOperations.Persistence \
  --startup-project src/LIMS.Service.LaboratoryOperations.API
```

Если `dotnet ef` отсутствует:

```bash
dotnet tool install --global dotnet-ef
```

После этого база данных будет приведена к актуальной схеме.

---

## 5. Заполнение базы данных

После применения миграций необходимо выполнить скрипт инициализации, находящийся в корне репозитория:

```cmd
run-lims.cmd
```

Скрипт предназначен для запуска сервисов проекта и автоматически подготавливает/запускает несколько приложений. В текущей версии он запускает:

* `Guides.Service`;
* `LIMS.Service.Methodologies.API`;
* `LIMS.Service.LaboratoryOperations.API`.

> **Важно:** перед запуском скрипта должны быть доступны PostgreSQL и RabbitMQ.

---

# Сервисы проекта

## Guides.Service

Сервис для статических данных, которые не несут в себе сложной бизнес-логики. Например: Ед. изм., Оборудование, Заказчики.

---

## LIMS.Service.Methodologies

Сервис отвечает за управление методиками испытаний.

Основные сущности:

```text
StudyTemplate
├── InputParameter
├── ResultDefinition
└── CalculationRule
```

Методика содержит:

* входные параметры;
* определяемые показатели;
* единицы измерения;
* спецификации;
* математические формулы;
* правила расчёта.

Persistence-слой сервиса использует собственный `ApplicationDbContext`, репозитории и Unit of Work.

---

## LIMS.Service.LaboratoryOperations

Сервис отвечает за транзакционные лабораторные операции:

```text
Order
└── Sample
    └── Study
        ├── MeasuredValue
        └── TestResult
```

Он работает независимо от контекста методик и использует собственный `ApplicationDbContext`.

---

# Жизненные циклы

## Жизненный цикл методики

```mermaid
stateDiagram-v2
    [*] --> Draft

    Draft --> Active : Утверждение (публикация snapshot)
    Draft --> [*] : Удаление

    Active --> Archived : Архивирование
```

---

## Жизненный цикл Задания (Order)

```mermaid
stateDiagram-v2
    [*] --> Draft

    Draft --> InProgress : Взятие в работу
    Draft --> Canceled : Отмена
    Draft --> [*] : Удаление

    InProgress --> Completed : Завершение
    InProgress --> Canceled : Отмена

    Completed --> [*]
    Canceled --> [*]
```

---

## Жизненный цикл Пробы (Sample)

```mermaid
stateDiagram-v2
    [*] --> Registered

    Registered --> InProgress : Начало исследований
    Registered --> Canceled : Отмена
    Registered --> [*] : Удаление

    InProgress --> Completed : Все исследования завершены
    InProgress --> Canceled : Отмена

    Completed --> [*]
    Canceled --> [*]
```

---

## Жизненный цикл Исследования (Study)

```mermaid
stateDiagram-v2
    [*] --> InProgress

    InProgress --> Completed : Завершение
    InProgress --> Canceled : Отмена
    InProgress --> [*] : Удаление

    Completed --> Approved : Утверждение
    Completed --> Canceled : Аннулирование

    Approved --> [*]
    Canceled --> [*]
```

---

# Оркестрация лабораторного процесса

Основной бизнес-процесс строится следующим образом:

```mermaid
flowchart LR
    A[Order<br/>Задание] -->|1:N| B[Sample<br/>Проба]
    B -->|1:N| C[Study<br/>Исследование]

    C -->|1:N| D[MeasuredValue<br/>Сырые измерения]
    C -->|1:N| E[TestResult<br/>Результаты]

    C -->|1:1| S[StudyTemplateSnapshot<br/>Снимок методики]
```

Важно понимать, что **snapshot методики создаётся не в момент создания `Study`, а значительно раньше — при публикации методики в сервисе `Methodologies`** (переход `Draft → Active`). В этот момент публикуется интеграционное событие `StudyTemplatePublishedMessage`, которое доставляется в `LaboratoryOperations`, где на его основе формируется отдельный агрегат `StudyTemplateSnapshot`.

Когда создаётся `Study`, он использует **уже существующий** snapshot из собственного контекста `LaboratoryOperations`. Это позволяет исследованию оставаться неизменным с точки зрения применённой методики даже после появления новой ревизии `StudyTemplate`.

---

# Бизнес-процессы

## Сценарий создания (п. 6.3.1 ГОСТ)

1. **Регистрация Задания** — создание бизнес-обязательства перед заказчиком.
2. **Регистрация Проб** — привязка физических образцов к заданию с указанием даты отбора, объёма и кода.
3. **Создание Исследований** — автоматическая генерация структуры исследования на основе **уже существующего** `StudyTemplateSnapshot`, ранее полученного через интеграционное событие:

  1. Создаются `MeasuredValue` для входных параметров.
  2. Создаются `TestResult` для определяемых показателей.
  3. Исторические данные методики копируются в исследование.

Таким образом, последующие изменения методики не изменяют уже созданные исследования.

---

## Конфигурирование методики (п. 8.6.2 ГОСТ)

Процесс конфигурирования методики:

1. Создание методики в статусе **Draft**.
2. Добавление входных параметров.
3. Добавление определяемых показателей.
4. Определение правил расчёта.
5. Привязка переменных формул к входным параметрам.
6. **Утверждение методики — переход `Draft → Active`**. В этот момент:
  * публикуется интеграционное событие `StudyTemplatePublishedMessage`;
  * `LaboratoryOperations` получает событие и создаёт собственный `StudyTemplateSnapshot`.

---

## Частичное обновление структуры

Пока методика находится в состоянии **Draft**, разрешено изменение её структуры.

Доступны:

* частичное обновление параметров;
* частичное обновление показателей;
* изменение правил расчёта;
* добавление элементов;
* удаление элементов;
* изменение связей между переменными и параметрами.

Для частичных изменений используется `PATCH`.

После перехода в `Active` прямое изменение структуры запрещено.

---

## Контроль изменений

Изменения мастер-данных контролируются через жизненный цикл методики.

|    Статус    | Изменение структуры |
| :----------: | :-----------------: |
|   **Draft**  |      Разрешено      |
|  **Active**  |      Запрещено      |
| **Archived** |      Запрещено      |

Для изменения `Active` или `Archived` методики должна создаваться новая ревизия.

---

# Движок расчётов

LIMS поддерживает автоматический расчёт результатов по формулам, определённым в `StudyTemplateSnapshot`.

```mermaid
sequenceDiagram
    participant A as Аналитик
    participant API as REST API
    participant S as Study
    participant MV as MeasuredValue
    participant TR as TestResult
    participant E as Formula Engine

    A->>API: PATCH /measured-values/{id}
    API->>MV: Update(35.5)

    A->>API: POST /test-results/{id}/execute

    API->>S: Загрузка Study + StudyTemplateSnapshot
    API->>TR: Поиск CalculationRule
    API->>MV: Сбор переменных
    API->>E: Evaluate(formula, variables)

    E-->>API: 14.2

    API->>TR: Update(14.2, IsOutOfSpec=false)
    API-->>A: 204 No Content
```

## Определение выхода за спецификацию

После вычисления результата система сравнивает полученное значение с `SpecMin` и `SpecMax`, которые были сохранены в `StudyTemplateSnapshot` **в момент публикации методики** (а не в момент создания исследования).

На основании этого выставляется флаг `IsOutOfSpec`.

Таким образом, проверка выполняется относительно спецификации, актуальной на момент публикации методики. Последующие ревизии методики не влияют на уже созданные исследования.

---

# Использованные архитектурные паттерны

## Domain-Driven Design

Основная архитектурная концепция проекта.

Бизнес-модель разделена на:

* Aggregates;
* Entities;
* Value Objects;
* Domain Services;
* Domain Rules;
* Bounded Contexts.

DDD используется не только как структура папок, но и как способ моделирования бизнес-инвариантов лабораторного процесса.

---

## Bounded Context

Проект разделён как минимум на два основных бизнес-контекста:

```text
StudyTemplateContext (Methodologies)
        │
        │ StudyTemplatePublishedMessage
        │ (через RabbitMQ + Outbox)
        ▼
LaboratoryOperationsContext
        │
        │ сохраняет
        ▼
StudyTemplateSnapshot (отдельный агрегат)
```

### StudyTemplateContext

Отвечает за мастер-данные:

* методики;
* параметры;
* результаты;
* формулы;
* спецификации.

### LaboratoryOperationsContext

Отвечает за транзакционные данные:

* задания;
* пробы;
* исследования;
* измерения;
* результаты;
* snapshots методик (полученные через интеграционные события).

---

## Aggregate

Агрегаты используются для определения границ изменения данных и защиты инвариантов.

Например:

```text
StudyTemplate
├── InputParameters
├── ResultDefinitions
└── CalculationRules
```

Изменение структуры агрегата происходит через его публичное поведение, а не через произвольное изменение дочерних сущностей из Application Layer.

---

## Repository Pattern

Репозитории используются для получения и сохранения Aggregate Roots.

Application Layer работает с абстракциями:

```text
IStudyRepository
IStudyTemplateRepository
IOrderRepository
ISampleRepository
```

Конкретная реализация находится в Persistence Layer.

---

## Unit of Work

`UnitOfWork` используется как граница сохранения изменений.

Application Layer:

```text
Load Aggregate
      ↓
Execute business operation
      ↓
UnitOfWork
      ↓
SaveChanges
```

Это позволяет объединять изменения нескольких сущностей в одну транзакцию базы данных.

---

## Domain Services

Domain Services используются для бизнес-операций, которые не принадлежат естественным образом одному Aggregate.

Например, когда бизнес-правило требует координации нескольких агрегатов (валидация перехода статуса `Order` с учётом статуса всех дочерних `Sample`, проверка возможности создания `Study` по текущему статусу `Order` и `Sample`).

При этом Application Layer отвечает за orchestration, а само бизнес-правило остаётся в Domain Layer.

---

## Snapshot Pattern

Один из ключевых паттернов проекта.

Snapshot создаётся **при публикации методики** (`Draft → Active`), а не при создании исследования:

```text
StudyTemplate v1 (Methodologies)
      │
      │ Draft → Active
      │ публикуется StudyTemplatePublishedMessage
      ▼
RabbitMQ (через Outbox)
      │
      │ доставляется в LaboratoryOperations
      ▼
StudyTemplateSnapshot (отдельный агрегат в LabOps)
      │
      │ используется при создании
      ▼
Study
```

Это позволяет:

* сохранять историческое состояние методики;
* не зависеть от последующих изменений мастер-данных;
* воспроизводить результаты исследований;
* обеспечивать аудит применённой методики;
* полностью развязать два bounded context (они не обращаются к БД друг друга).

---

## Outbox Pattern

Для надёжной публикации интеграционных событий используется **Transactional Outbox**.

Вместо:

```text
Database
   ↓
RabbitMQ
```

используется:

```text
┌───────────────────────────┐
│       DB Transaction      │
│                           │
│  Business Entity          │
│  OutboxMessage            │
│                           │
└─────────────┬─────────────┘
              │ COMMIT
              ▼
       Outbox Processor
              │
              ▼
          RabbitMQ
```

Бизнес-изменение и `OutboxMessage` сохраняются в одной транзакции.

После успешного commit фоновый worker публикует сообщение в RabbitMQ. Если брокер недоступен — сообщение остаётся в таблице и будет отправлено при следующей итерации.

Outbox вынесен в переиспользуемую библиотеку и может работать с разными `DbContext`, поэтому каждый микросервис может использовать собственную persistence-модель.

---

## Integration Events

Для коммуникации между микросервисами используются интеграционные события.

Например:

```text
StudyTemplatePublishedMessage
UnitCreatedMessage
```

Сервис-источник:

```text
Domain/Application
       ↓
Outbox
       ↓
RabbitMQ
```

Сервис-получатель:

```text
RabbitMQ
       ↓
Message Handler
       ↓
Application
       ↓
Сохранение StudyTemplateSnapshot
```

Таким образом, бизнес-контексты не должны напрямую обращаться к базам данных друг друга.

---

## Asynchronous Messaging

RabbitMQ используется для асинхронного взаимодействия между сервисами.

Это позволяет отделить:

```text
Producer
   │
   ▼
Message Broker
   │
   ▼
Consumer
```

и уменьшить связанность между микросервисами.

RabbitMQ инфраструктура также вынесена в отдельные библиотеки:

```text
RabbitMq.Library.Broker
RabbitMq.Library.Broker.Abstractions
RabbitMq.Library.Outbox
RabbitMq.Library.Outbox.Abstractions
```

---

## Event-Driven Architecture

Интеграционные события используются для уведомления других bounded contexts о произошедших изменениях.

```mermaid
sequenceDiagram
    participant M as Methodologies
    participant O as Outbox
    participant R as RabbitMQ
    participant L as Laboratory Operations

    M->>O: StudyTemplatePublished
    O->>R: Publish message
    R->>L: Deliver message
    L->>L: Сохранить StudyTemplateSnapshot
```

При этом RabbitMQ не участвует в основной транзакции бизнес-операции.

---

## State Machine

Управление жизненным циклом агрегатов реализовано через State Machine Pattern. Каждый статус (`Draft`, `Active`, `Archived`, `InProgress`, `Completed` и т.д.) определяет:

* допустимые переходы (`CanTransitionTo`);
* возможность редактирования (`CanEdit`);
* возможность создания дочерних сущностей (`CanAcceptNewEntity`);
* возможность удаления связанных сущностей (`CanDeleteAssociatedEntities`).

Правила переходов инкапсулированы в самих сущностях статусов, а не размазаны по коду бизнес-логики.

---

## Result Pattern

Вместо исключений для управления потоком ошибок используется `Result<TValue, TError>` — явное представление успеха или неудачи.

Пример из домена:

```csharp
public static Result<Name, DomainError> Create(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        return new ValidationError("Name cannot be empty.");

    if (value.Length > MaxLength)
        return new ValidationError($"Name cannot exceed {MaxLength} characters.");

    return new Name(value.Trim());
}
```

**Преимущества:**

* Явная сигнатура метода показывает возможные ошибки;
* Компилятор контролирует обработку ошибок;
* Отсутствие исключений для ожидаемых сценариев;
* Легко комбинировать через LINQ-подобные операции.

---

## Двухуровневая модель ошибок

В проекте используются два уровня ошибок для чёткого разделения ответственности:

### Domain Errors

Ошибки бизнес-логики, возникающие в доменном слое:

```csharp
public abstract record DomainError
{
    public abstract string Code { get; }
    public abstract string Message { get; }
}
```

Примеры: `EntityNotFoundError`, `EntityAlreadyDeletedError`, `InvalidStatusTransitionError`, `EntityNotEditableError`, `EntityInUseError`, `ValidationError`.

### Application Errors

Ошибки application-слоя, оборачивающие доменные ошибки или представляющие инфраструктурные проблемы:

```csharp
public abstract record ApplicationError;

public sealed record DomainRuleViolation(DomainError Error) : ApplicationError;
public sealed record NotFoundError(string Message) : ApplicationError;
public sealed record ValidationError(string Message) : ApplicationError;
public sealed record PersistenceError(string Message) : ApplicationError;
```

API-слой маппит `ApplicationError` на HTTP-статусы:

| Тип ошибки | HTTP статус |
|------------|-------------|
| `NotFoundError` / `EntityNotFoundError` | 404 Not Found |
| `ValidationError` | 400 Bad Request |
| `InvalidStatusTransitionError` / `EntityNotEditableError` / `EntityInUseError` | 409 Conflict |
| `PersistenceError` | 500 Internal Server Error |

---

## Soft Delete

Для мастер-данных используется Soft Delete:

```text
IsDeleted = true
DeletedAt = DateTimeOffset.UtcNow
```

Физическое удаление не производится, что позволяет:

* сохранять историю;
* поддерживать аудит;
* не нарушать историческую целостность связанных данных.

При удалении агрегата каскадно помечаются удалёнными все его дочерние сущности.

---

## Optimistic Concurrency

При изменении данных используется подход, позволяющий обнаруживать конфликтующие изменения и не допускать незаметного перезаписывания данных другим запросом.

---

# Архитектурные принципы

## 1. Разделение контекстов

```text
StudyTemplateContext (Methodologies)
        │
        │ StudyTemplatePublishedMessage
        │ (через Outbox + RabbitMQ)
        ▼
LaboratoryOperationsContext
```

Контексты имеют собственные модели и базы данных.

Они не используют прямые EF Core relationships между агрегатами разных bounded contexts.

---

## 2. Инкапсуляция бизнес-логики

### Application Layer

Отвечает за orchestration:

* загрузку Aggregate;
* вызов бизнес-операций;
* координацию use case;
* Unit of Work.

### Domain Layer

Отвечает за:

* бизнес-правила;
* инварианты;
* переходы состояний;
* создание дочерних сущностей;
* Domain Services.

### Infrastructure / Persistence

Отвечают за технические детали:

* PostgreSQL;
* EF Core;
* RabbitMQ;
* Outbox;
* внешние интеграции.

---

## 3. Dependency Inversion

Application и Domain не должны зависеть от конкретных инфраструктурных реализаций.

Например:

```text
Application
     │
     ▼
IStudyRepository
     ▲
     │
Persistence
```

А не:

```text
Application
     │
     ▼
StudyRepository
     │
     ▼
EF Core
```

---

## 4. Transactional Boundaries

Изменение бизнес-сущности и соответствующего Outbox-сообщения выполняется в рамках одной транзакции:

```text
BEGIN
   │
   ├── Update business data
   │
   ├── Insert OutboxMessage
   │
COMMIT
```

После commit Outbox worker независимо от HTTP-запроса доставляет интеграционное событие в брокер.

---

# Технологии

| Технология                   | Назначение                        |
| ---------------------------- | --------------------------------- |
| **C# / .NET 10**             | Основной runtime                  |
| **ASP.NET Core**             | REST API                          |
| **Entity Framework Core 10** | ORM                               |
| **PostgreSQL**               | Хранение данных                   |
| **RabbitMQ**                 | Message Broker                    |
| **Docker**                   | Инфраструктура                    |
| **Carter**                   | Организация Minimal API endpoints |
| **Swagger / OpenAPI**        | API documentation                 |
| **Mermaid**                  | Архитектурные и бизнес-диаграммы  |
| **Git**                      | Version Control                   |

---

# Быстрый запуск

Если окружение уже настроено, последовательность запуска выглядит следующим образом:

```bash
# 1. Клонировать
git clone https://github.com/SaintEfim/LIMS.DDD.git
cd LIMS.DDD
```

```bash
# 2. Запустить RabbitMQ
docker run -it --rm \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

```bash
# 3. Применить миграции
dotnet ef database update \
  --project src/LIMS.Service.Methodologies.Persistence \
  --startup-project src/LIMS.Service.Methodologies.API

dotnet ef database update \
  --project src/LIMS.Service.LaboratoryOperations.Persistence \
  --startup-project src/LIMS.Service.LaboratoryOperations.API

dotnet ef database update \
  --project src/LIMS.Service.Guides \
  --startup-project src/LIMS.Service.Guides
```

Затем:

```cmd
run-lims.cmd
```

После этого должны быть запущены:

```text
Guides.Service
LIMS.Service.Methodologies.API
LIMS.Service.LaboratoryOperations.API
```

И RabbitMQ:

```text
AMQP:       localhost:5672
Management: localhost:15672
```

## Демонстрационный сценарий

Файл [`LIMS.http`](./LIMS.http) содержит последовательность запросов, которая демонстрирует полный жизненный цикл лабораторного процесса на примере определения влажности зерна — от настройки методики до завершения исследования.

### Порядок выполнения

1. **Создание единиц измерения**

   Регистрируются единицы измерения, необходимые для работы с методикой:
   - `г` — грамм;
   - `%` — процент.

2. **Создание методики**

   Создаётся методика определения влажности зерна.

   В методике определяются три входных параметра:
   - `m1` — масса бюксы с зерном до высушивания;
   - `m2` — масса бюксы с зерном после высушивания;
   - `m0` — масса пустой бюксы.

3. **Определение показателей**

   В методику добавляются определяемые показатели:
   - **Вода**;
   - **Сухое вещество**.

   Для показателей задаются единицы измерения и допустимые диапазоны значений.

4. **Настройка правила расчёта**

   Для определения содержания воды задаётся формула:

   ```text
   ((m1 - m2) / (m1 - m0)) * 100
    ````

Переменные формулы связываются с соответствующими входными параметрами методики.

5. **Утверждение методики**

   Методика переводится из состояния **Draft** в **Active**.

   После утверждения её структура становится неизменяемой. Для дальнейших изменений необходимо создать новую ревизию.

6. **Создание новой ревизии**

   Создаётся новая ревизия методики, демонстрируя механизм версионирования и контроля изменений.

7. **Регистрация задания**

   Создаётся лабораторное задание на проведение исследования.

8. **Регистрация пробы**

   В рамках задания регистрируется проба зерна с необходимыми характеристиками.

9. **Создание исследования**

   Для зарегистрированной пробы создаётся исследование с использованием утверждённой методики.

   В исследовании формируется набор входных измерений и определяемых показателей, соответствующий выбранной методике.

10. **Ввод результатов измерений**

    В исследование вносятся фактические значения:

  * `m1` — масса бюксы с зерном до высушивания;
  * `m2` — масса бюксы с зерном после высушивания;
  * `m0` — масса пустой бюксы.

11. **Расчёт результатов**

    На основании введённых измерений рассчитывается содержание воды в зерне.

    Полученное значение сравнивается с допустимой спецификацией, после чего определяется, находится ли результат в пределах нормы.

12. **Завершение исследования**

    После выполнения всех необходимых операций исследование переводится в состояние **Completed**.

---

# Нормативная основа

Предметная область и основные бизнес-процессы проекта моделировались с опорой на:

**ГОСТ Р 53798-2010  
«Стандартное руководство по лабораторным информационным менеджмент-системам (ЛИМС)».**

ГОСТ используется как источник требований при моделировании жизненных циклов, процессов работы с заданиями, пробами, исследованиями, методиками и результатами.

При этом архитектура приложения является самостоятельной разработкой и включает DDD, Bounded Contexts, Aggregates, Snapshot Pattern, Transactional Outbox и событийное взаимодействие между контекстами.

# Цель проекта

Основная цель проекта — не создание production-ready LIMS, а практическое исследование того, как можно моделировать сложную предметную область с использованием:

* **Domain-Driven Design**
* **Bounded Contexts**
* **Aggregates**
* **Domain Services**
* **Repository Pattern**
* **Unit of Work**
* **Snapshot Pattern**
* **Transactional Outbox**
* **Integration Events**
* **RabbitMQ**
* **Clean Architecture**
* **Microservices**
* **Event-Driven Architecture**
* **Result Pattern**
* **State Machine**

Проект постепенно развивается в сторону более полноценной распределённой архитектуры, где каждый bounded context обладает собственной моделью данных и отвечает за собственную бизнес-область.
