# Event Driven Algorithm Solver

## Description
The **Event Driven Algorithm Solver** is a distributed system that allows users to submit inputs for algorithmic problems through a web application. The system processes these problems asynchronously using RabbitMQ for messaging, worker applications for computation, and an Azure Function App for sending email notifications with results.

## Table of Contents
- [Architecture Overview](#architecture-overview)
- [Features](#features)
- [Technologies](#technologies)

## Architecture Overview
The system is composed of the following components:

1. **Web Application (ASP.NET Core MVC)**
   - Handles user authentication (registration & login).
   - Allows users to submit algorithmic problem inputs via forms (1 form / 1 algorithm).
   - Publishes input data to a RabbitMQ exchange for processing.

2. **Message Broker (RabbitMQ)**
   - Manages messaging between the web application and worker applications.
   - Uses topic exchange and queues for distributing tasks to worker apps.
   - Each algorithm has its own queue and 1 or more workers can listen to said queue.

3. **Worker Applications**
   - Listen to RabbitMQ queues for algorithm problem submissions.
   - Process the input data by executing the appropriate algorithm.
   - References an assembly (class library) which implements said algorithm and feeds the user input to it
   - Send computed results (input/output) to an Azure Function App.

4. **Azure Function App**
   - Receives processed results from worker applications.
   - Emails the results to the user who submitted the problem.

## Features

- **User Authentication**:
  - Secure user authentication using ASP.NET Core MVC to manage users and their algorithm-solving requests.

- **Problem Submission**:
  - A web interface for users to submit algorithm-solving problems, including problem details and input data.

- **Event-Driven Architecture**:
  - Utilizes RabbitMQ for distributing algorithm-solving tasks between workers, ensuring a decoupled, scalable, and efficient system.

- **Task Processing Workers**:
  - Dedicated worker applications that listen for tasks and process algorithm-solving jobs asynchronously.

- **Real-Time Algorithm Solving**:
  - Tasks are solved in real time by worker apps, which process input data and compute results based on the submitted algorithms.

- **Email Notifications**:
  - An Azure Function App that sends email notifications with the results once an algorithm has been successfully solved.

- **Scalable Design**:
  - The architecture is designed to scale easily by adding more worker applications to handle an increased load of algorithm-solving tasks.

- **Cloud-Ready**:
  - The system is hosted on Azure, ensuring reliability and availability in a cloud environment.


## Technologies
- **.NET 8 (ASP.NET Core MVC, Worker Services)**
- **RabbitMQ** (Message Queue)
- **Azure Functions** (Serverless Computing)
- **Entity Framework Core** (Database ORM)
- **SQLite** (Database for storing user data and submissions)
- **Docker** (For containerized deployment)
