# System Architecture – MediLink


MediLink Architecture Components

• Patient UI (registration, booking)
• Doctor UI (view schedule)
• Admin dashboard
• Technologies (example):
• HTML, CSS, JavaScript

Purpose: Business logic
• Authentication & authorization
• Appointment scheduling logic
• SMS notification handling
• Admin management



REST APIs

• Purpose: Data storage
• User data
• Doctor profiles & schedules
• Appointments
• Admin records
• MySQL Server

Services

• Purpose: Communication
• SMS Gateway (Twilio)



## Architecture Overview
MediLink follows a 3-tier architecture consisting of Presentation, Application, and Data layers.

## Presentation Layer
This layer provides user interfaces for patients, doctors, and administrators. Users can register, log in, book appointments, and manage schedules.

## Application Layer
The application layer handles business logic such as authentication, appointment scheduling, SMS notifications, and admin management through REST APIs.

## Data Layer
This layer stores all system data including user details, doctor profiles, appointment records, and system logs.

## External Services
An SMS gateway is integrated to send appointment confirmations and reminders to users.

## Advantages
- Scalable and modular design
- Secure data handling
- Easy maintenance
