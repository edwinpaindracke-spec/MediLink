-- Ensure Hospitals table exists
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Hospitals' AND xtype = 'U')
BEGIN
    CREATE TABLE Hospitals (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Address NVARCHAR(250) NOT NULL
    );
END


-- Makotopong Hospital (Primary)
IF NOT EXISTS (SELECT 1 FROM Hospitals WHERE Name = 'Makotopong Hospital')
BEGIN
    INSERT INTO Hospitals (Name, Address)
    VALUES ('Makotopong Hospital', 'Makotopong Village, Molemole, Limpopo');
END

-- Insert hospitals in Polokwane, Limpopo (only if they don't already exist)
IF NOT EXISTS (SELECT 1 FROM Hospitals WHERE Name = 'Polokwane Provincial Hospital')
BEGIN
    INSERT INTO Hospitals (Name, Address)
    VALUES ('Polokwane Provincial Hospital', 'Hospital Street, Polokwane, Limpopo');
END

IF NOT EXISTS (SELECT 1 FROM Hospitals WHERE Name = 'Mediclinic Limpopo')
BEGIN
    INSERT INTO Hospitals (Name, Address)
    VALUES ('Mediclinic Limpopo', '101 Rabe Street, Polokwane, Limpopo');
END

IF NOT EXISTS (SELECT 1 FROM Hospitals WHERE Name = 'Netcare Pholoso Hospital')
BEGIN
    INSERT INTO Hospitals (Name, Address)
    VALUES ('Netcare Pholoso Hospital', 'Silwerkruin, Polokwane, Limpopo');
END
