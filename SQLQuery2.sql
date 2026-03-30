
CREATE TABLE designation(
	Id INT PRIMARY KEY IDENTITY,
	designationName NVARCHAR(30)
)
GO

INSERT INTO designation VALUES
('Assistant Professor'),
('Associate Professor'),
('Lecturer'),
('Consultant'),
('Sub Registrar')
GO


CREATE TABLE Doctors(
	doctorId INT PRIMARY KEY IDENTITY,
	doctorName NVARCHAR (50) NOT NULL,
	email NVARCHAR(30),
	contactNumber NVARCHAR(30) NOT NULL,
	designationId INT REFERENCES designation(Id) NOT NULL,
	chamberInDhaka BIT,
	picture NVARCHAR(50)
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
INSERT INTO Specialities(doctorId,specialities,description) VALUES(1,'Cardiology', 'Diagnoses and treats heart diseases'),
(1, 'Cardiac Surgery', 'Performs bypass and valve surgeries')
GO




SELECT * FROM Doctors 
SELECT Id,designationName FROM designation 
SELECT * FROM Specialities
SELECT SCOPE_IDENTITY()