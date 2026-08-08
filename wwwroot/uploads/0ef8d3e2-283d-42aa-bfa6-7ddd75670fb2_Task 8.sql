CREATE TABLE Doctor (
    DoctorID INT PRIMARY KEY,
    Name VARCHAR(100),
    Email VARCHAR(100),
    Phone VARCHAR(20),
    Specialty VARCHAR(100),
    YearsExperience INT
);

CREATE TABLE Patient (
    URNumber INT PRIMARY KEY,
    Name VARCHAR(100),
    Address VARCHAR(200),
    Age INT,
    Email VARCHAR(100),
    Phone VARCHAR(20),
    MedicareCardNo VARCHAR(50),
    PrimaryDoctorID INT NOT NULL,
    FOREIGN KEY (PrimaryDoctorID)
        REFERENCES Doctor(DoctorID)
);

CREATE TABLE PharmaceuticalCompany (
    CompanyID INT PRIMARY KEY,
    CompanyName VARCHAR(100),
    Address VARCHAR(200),
    Phone VARCHAR(20)
);

CREATE TABLE Drug (
    DrugID INT PRIMARY KEY,
    TradeName VARCHAR(100),
    Strength VARCHAR(50),
    CompanyID INT NOT NULL,
    FOREIGN KEY (CompanyID)
        REFERENCES PharmaceuticalCompany(CompanyID)
        ON DELETE CASCADE
);

CREATE TABLE Prescription (
    PrescriptionID INT PRIMARY KEY,
    PrescriptionDate DATE,
    Quantity INT,
    URNumber INT,
    DoctorID INT,
    DrugID INT,
    FOREIGN KEY (URNumber)
        REFERENCES Patient(URNumber),
    FOREIGN KEY (DoctorID)
        REFERENCES Doctor(DoctorID),
    FOREIGN KEY (DrugID)
        REFERENCES Drug(DrugID)



-- ADD CITY COLUMN

ALTER TABLE Patient
ADD City VARCHAR(100);

-- SELECT retrieve all columns from the Doctor table

SELECT *
FROM Doctor;

-- ORDER BY list patients in ascending order of their ages

SELECT *
FROM Patient
ORDER BY Age ASC;

-- OFFSET FETCH retrieve the first 10 patients starting from the 5th record

SELECT *
FROM Patient
ORDER BY URNumber
OFFSET 4 ROWS
FETCH NEXT 10 ROWS ONLY;

-- SELECT TOP retrieve the top 5 doctors

SELECT TOP 5 *
FROM Doctor;

-- SELECT DISTINCT get a list of unique addresses from the Patient table

SELECT DISTINCT Address
FROM Patient;

-- WHERE retrieve patients who are aged 25

SELECT *
FROM Patient
WHERE Age = 25;



-- NULL retrieve patients whose email is not provided

SELECT *
FROM Patient
WHERE Email IS NULL;

-- AND retrieve doctors with more than 5 years experience and specialize in Cardiology

SELECT *
FROM Doctor
WHERE YearsExperience > 5
AND Specialty = 'Cardiology';



-- INn retrieve doctors whose specialty is either dermatology or Oncology

SELECT *
FROM Doctor
WHERE Specialty IN ('Dermatology', 'Oncology');

-- BETWEEN retrieve patients whose ages are between 18 and 30

SELECT *
FROM Patient
WHERE Age BETWEEN 18 AND 30;

-- LIKE retrieve doctors whose names start with 'Dr.'

SELECT *
FROM Doctor
WHERE Name LIKE 'Dr.%';

-- COLUMN & TABLE ALIASES

SELECT
    d.Name AS DoctorName,
    d.Email AS DoctorEmail
FROM Doctor AS d;

-- JOIN retrieve all prescriptions with corresponding patient names

SELECT
    pr.PrescriptionID,
    p.Name AS PatientName,
    pr.PrescriptionDate,
    pr.Quantity
FROM Prescription AS pr
INNER JOIN Patient AS p
ON pr.URNumber = p.URNumber;

-- GROUP BY retrieve the count of patients grouped by their cities

SELECT
    City,
    COUNT(*) AS NumberOfPatients
FROM Patient
GROUP BY City;

-- HAVING retrieve cities with more than 3 patients

SELECT
    City,
    COUNT(*) AS NumberOfPatients
FROM Patient
GROUP BY City
HAVING COUNT(*) > 3;

-- EXISTS retrieve patients who have at least one prescription

SELECT *
FROM Patient p
WHERE EXISTS
(
    SELECT 1
    FROM Prescription pr
    WHERE pr.URNumber = p.URNumber
);

-- UNIONn retrieve a combined list of doctors and patients

SELECT Name, Email
FROM Doctor

UNION

SELECT Name, Email
FROM Patient;

-- INSERT Insert a new doctor

INSERT INTO Doctor
(
    DoctorID,
    Name,
    Email,
    Phone,
    Specialty,
    YearsExperience
)
VALUES
(
    101,
    'Dr. Ahmed Ali',
    'ahmed@example.com',
    '01234567890',
    'Cardiology',
    12
);

-- INSERT MULTIPLE ROWS Insert multiple patients

INSERT INTO Patient
(
    URNumber,
    Name,
    Address,
    City,
    Age,
    Email,
    Phone,
    MedicareCardNo,
    PrimaryDoctorID
)
VALUES
(1,'Mohamed Hassan','15 Nile St','Cairo',24,'mohamed@gmail.com','01011111111','MC1001',101),
(2,'Sara Ali','20 Sea Rd','Alexandria',30,'sara@gmail.com','01022222222','MC1002',101),
(3,'Omar Mahmoud','10 Pyramids St','Giza',20,NULL,'01033333333',NULL,101);

-- UPDATE Update the phone number of a doctor.

UPDATE Doctor
SET Phone = '01199999999'
WHERE DoctorID = 101;

-- UPDATE JOIN Update the city of patients who have a prescription from a specific doctor

UPDATE p
SET City = 'Melbourne'
FROM Patient p
INNER JOIN Prescription pr
ON p.URNumber = pr.URNumber
WHERE pr.DoctorID = 101;

-- DELETE Delete a patient

DELETE FROM Patient
WHERE URNumber = 3;

-- TRANSACTION Insert a doctor and a patient together

BEGIN TRANSACTION;

BEGIN TRY

    INSERT INTO Doctor
    (
        DoctorID,
        Name,
        Email,
        Phone,
        Specialty,
        YearsExperience
    )
    VALUES
    (
        102,
        'Dr. John Smith',
        'john@example.com',
        '01288888888',
        'Dermatology',
        8
    );

    INSERT INTO Patient
    (
        URNumber,
        Name,
        Address,
        City,
        Age,
        Email,
        Phone,
        MedicareCardNo,
        PrimaryDoctorID
    )
    VALUES
    (
        10,
        'Ali Hassan',
        '25 Main St',
        'Cairo',
        28,
        'ali@gmail.com',
        '01044444444',
        'MC1010',
        102
    );

    COMMIT TRANSACTION;

END TRY

BEGIN CATCH

    ROLLBACK TRANSACTION;

END CATCH;
);