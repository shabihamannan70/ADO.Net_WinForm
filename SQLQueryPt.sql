CREATE DATABASE projectdb
GO
USE	projectdb
GO

CREATE TABLE designation(
	Id INT PRIMARY KEY IDENTITY,
	designationName NVARCHAR(30)
)
GO

INSERT INTO designation VALUES
('Assistant Professor'),
('Associate Professor')
GO


CREATE TABLE Doctors(
	doctorId INT PRIMARY KEY IDENTITY,
	doctorName NVARCHAR (50) NOT NULL,
	email NVARCHAR(30),
	contactNumber NVARCHAR(30) NOT NULL,
	designationId INT REFERENCES designation(Id) NOT NULL,
	chamberInDhaka BIT,
	picture NVARCHAR(50),
	photo VARBINARY(MAX)
)
GO

INSERT INTO Doctors(doctorName,email,contactNumber,designationId,chamberInDhaka,picture) VALUES('Dr. Rahman','Rahman12@gmail.com','01323555498',1,1,'a.png')
GO

CREATE TABLE Specialities(
	specialityId INT PRIMARY KEY IDENTITY,
	doctorId INT REFERENCES Doctors(doctorId),
	specialities NVARCHAR (50) NOT NULL,
	description NVARCHAR(100),
	fee money
)
GO

SELECT * FROM Doctors 
SELECT Id,designationName FROM designation 
SELECT doctorId,specialities,description,fee FROM Specialities


CREATE TABLE cities
(
	id INT PRIMARY KEY IDENTITY,
	cityName VARCHAR(20)
)
GO	
CREATE TABLE patients
(
	patientId INT IDENTITY PRIMARY KEY,
	name VARCHAR(20) NOT NULL,
	cityId INT NOT NULL REFERENCES cities(id),
	contact VARCHAR (20),
	gender VARCHAR(6) NOT NULL
)
GO
--SP
CREATE PROC spPatient
(
	@patientId INT=NULL,
	@name VARCHAR(20)=NULL,
	@cityId INT=NULL,
	@contact VARCHAR (20)=NULL,
	@gender VARCHAR(6)=NULL,
	@actionType VARCHAR(25)
)
AS
BEGIN
	IF @actionType='SaveData'
		BEGIN
			IF NOT EXISTS (SELECT * FROM patients WHERE patientId=@patientId)
			BEGIN
				INSERT INTO patients(name,cityId,contact,gender)
				VALUES(@name,@cityId,@contact,@gender)
			END
			ELSE
			BEGIN
				UPDATE patients SET name=@name,cityId=@cityId,contact=@contact,gender=@gender
				WHERE patientId=@patientId
			END
		END
	IF @actionType='DeleteData'
	BEGIN
		DELETE FROM patients WHERE patientId=@patientId
	END
	IF @actionType='FetchData'
	BEGIN
		SELECT p.patientId,p.name,p.gender,p.contact,c.cityName FROM patients p
		INNER JOIN cities c ON p.cityId=c.id
	END
	IF @actionType='FetchRecord'
	BEGIN
		SELECT p.patientId,p.name,p.gender,p.cityId,p.contact,c.cityName FROM patients p
		INNER JOIN cities c ON p.cityId=c.id
		WHERE patientId=@patientId
	END
END
GO
INSERT INTO cities VALUES('Dhaka'),
('Rajshahi'),
('Pabna'),
('Khulna'),
('Barishal'),
('Dinazpur'),
('Kushtia'),
('Natore')

SELECT * FROM patients


SELECT d.*,s.designationName FROM Doctors d
INNER JOIN designation s ON d.designationId=s.Id