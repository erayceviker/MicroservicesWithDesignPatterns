# Mikroservis Mimarisi ve Saga Tasarım Deseni

Bu proje, .NET ortamında mikroservis mimarisini kullanan dağıtık sistemlerde veri tutarlılığını sağlamak için **Saga tasarım deseninin** iki farklı yaklaşımını göstermek amacıyla geliştirilmiştir. Repozitör, her biri farklı bir Saga implementasyonunu içeren iki ayrı branch barındırmaktadır.

## Branch Yapısı ve Saga Implementasyonları

Bu repozitör, Saga deseninin iki temel yaklaşımını ayrı branch'lerde sunmaktadır:

*   **orchestration-saga branch'i**: Bu branch, **Orkestrasyon (Orchestration)** tabanlı Saga desenini içerir. Bu yaklaşımda, tüm süreç merkezi bir orkestratör (`SagaStateMachine`) tarafından yönetilir. Orkestratör, hangi işlemin yapılacağını, hangi servisin çağrılacağını ve hata durumunda hangi telafi edici işlemlerin tetikleneceğini bilir.

*   **choreography-saga branch'i**: Bu branch, **Koreografi (Choreography)** tabanlı Saga desenini içerir. Bu yaklaşımda merkezi bir yönetici yoktur. Servisler, birbirlerinin yayınladığı olayları (events) dinleyerek ne yapacaklarına karar verirler ve gevşek bağlı (loosely coupled) bir şekilde iletişim kurarlar.

> İlgilendiğiniz implementasyonu incelemek için lütfen ilgili branch'e geçiş yapınız.

## Kullanılan Teknolojiler

*   **.NET 9**
*   **ASP.NET Core:** Web API servisleri için.
*   **Entity Framework Core:** Veritabanı işlemleri için (SQL Server & In-Memory).
*   **MassTransit:** Servisler arası asenkron iletişimi sağlayan, RabbitMQ üzerinde çalışan bir servis otobüsü.
*   **RabbitMQ:** Mesaj kuyruğu sistemi.
*   **SQL Server:** Sipariş ve Saga durum verilerini kalıcı olarak saklamak için.
*   **Swagger/OpenAPI:** API'leri test etmek ve belgelemek için.

## Kurulum ve Başlatma

1.  **Ön Gereksinimler:**
    *   .NET 9 SDK
    *   RabbitMQ
    *   SQL Server (LocalDB yeterlidir)

2.  **Yapılandırma:**
    *   `appsettings.Development.json` dosyalarındaki `ConnectionStrings` ayarlarını kendi RabbitMQ ve SQL Server bilgilerinize göre güncelleyin.

3.  **Veritabanı Kurulumu:**
    *   Package Manager Console (PMC) üzerinden aşağıdaki komutları çalıştırarak veritabanlarını oluşturun:
        ```powershell
        # Order.Api projesi için
        Update-Database -Project Order.Api

        # SagaStateMachineWorkerService projesi için (orkestrasyon branch'inde)
        Update-Database -Project SagaStateMachineWorkerService
        ```

4.  **Uygulamaları Başlatma:**
    *   Tüm projeleri (`Order.Api`, `Stock.Api`, `Payment.Api` ve varsa `SagaStateMachineWorkerService`) aynı anda başlatın.

---
---

# Microservices Architecture & Saga Design Pattern

This project was developed to demonstrate two different approaches to the **Saga design pattern** for ensuring data consistency in distributed systems using a microservice architecture with .NET. The repository contains two separate branches, each featuring a different Saga implementation.

## Branch Structure & Saga Implementations

This repository presents the two primary approaches of the Saga pattern in separate branches:

*   **orchestration-saga branch**: This branch contains the **Orchestration-based** Saga pattern. In this approach, the entire process is managed by a central orchestrator (`SagaStateMachine`). The orchestrator knows which operations to execute, which services to call, and which compensating transactions to trigger in case of a failure.

*   **choreography-saga branch**: This branch contains the **Choreography-based** Saga pattern. In this approach, there is no central coordinator. Services decide what to do by listening to events published by other services, communicating in a loosely coupled manner.

> Please check out the respective branch to review the implementation you are interested in.

## Technologies Used

*   **.NET 9**
*   **ASP.NET Core:** For Web API services.
*   **Entity Framework Core:** For database operations (SQL Server & In-Memory).
*   **MassTransit:** A service bus for .NET that provides an abstraction layer over RabbitMQ for asynchronous inter-service communication.
*   **RabbitMQ:** The message broker.
*   **SQL Server:** For persisting order and Saga state data.
*   **Swagger/OpenAPI:** For documenting and testing the APIs.

## Getting Started

1.  **Prerequisites:**
    *   .NET 9 SDK
    *   RabbitMQ
    *   SQL Server (LocalDB is sufficient)

2.  **Configuration:**
    *   Update the `ConnectionStrings` in the `appsettings.Development.json` files with your own RabbitMQ and SQL Server information.

3.  **Database Setup:**
    *   Run the following commands in the Package Manager Console (PMC) to create the databases:
        ```powershell
        # For the Order.Api project
        Update-Database -Project Order.Api

        # For the SagaStateMachineWorkerService project (in the orchestration branch)
        Update-Database -Project SagaStateMachineWorkerService
        ```

4.  **Running the Applications:**
    *   Start all projects (`Order.Api`, `Stock.Api`, `Payment.Api`, and `SagaStateMachineWorkerService` if applicable) simultaneously.
