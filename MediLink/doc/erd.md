erDiagram 
    USER {
        int user_id PK
        string name
        string email
        string phone
        string password
    }

    DOCTOR {
        int doctor_id PK
        string name
        string specialization
        string phone
        string schedule
    }

    APPOINTMENT {
        int appointment_id PK
        date appointment_date
        time appointment_time
        string status
        int user_id FK
        int doctor_id FK
    }

    ADMIN {
        int admin_id PK
        string username
        string password
    }

    NOTIFICATION {
        int notification_id PK
        string message
        datetime sent_time
        int appointment_id FK
    }

    USER ||--o{ APPOINTMENT : books
    DOCTOR ||--o{ APPOINTMENT : attends
    APPOINTMENT ||--|| NOTIFICATION : triggers
    ADMIN ||--o{ DOCTOR : manages
    ADMIN ||--o{ APPOINTMENT : oversees
