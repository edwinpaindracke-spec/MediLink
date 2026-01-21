# Entity Relationship Diagram (ERD) – MediLink

## Overview
This folder contains the Entity Relationship Diagram (ERD) for **MediLink**, a doctor appointment booking system.  
The ERD represents the database structure of the system, showing entities, their attributes, and relationships between them.

The diagram helps in understanding how data is stored, accessed, and related within the MediLink application.

---

## Purpose of ERD
The ERD is designed to:
- Define the database structure clearly
- Identify entities and their relationships
- Support backend development and database design
- Serve as documentation for academic and project evaluation purposes

---

## Entities Description

### 1. User
Represents patients who use the system to book appointments.
- user_id (Primary Key)
- name
- email
- phone
- password

---

### 2. Doctor
Represents doctors registered in the system.
- doctor_id (Primary Key)
- name
- specialization
- phone
- availability

---

### 3. Appointment
Represents appointment bookings between users and doctors.
- appointment_id (Primary Key)
- appointment_date
- appointment_time
- status
- user_id (Foreign Key)
- doctor_id (Foreign Key)

---

### 4. Admin
Represents system administrators who manage the application.
- admin_id (Primary Key)
- username
- password

---

### 5. Notification
Represents SMS notifications sent to users.
- notification_id (Primary Key)
- message
- sent_time
- appointment_id (Foreign Key)

---

## Relationships

- A **User** can book multiple **Appointments**
- A **Doctor** can receive multiple **Appointments**
- Each **Appointment** generates one **Notification**
- An **Admin** manages doctors and appointments

---