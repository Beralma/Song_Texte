USE master;
GO

IF DB_ID(N'SongTexte') IS NULL
  CREATE DATABASE SongTexte;
GO

USE SongTexte;
GO 

DROP TABLE IF EXISTS Ausgabe;
DROP TABLE IF EXISTS Stil;
DROP TABLE IF EXISTS Thema;
DROP TABLE IF EXISTS Login;


CREATE TABLE Login (
  Login_id INT IDENTITY (1, 1) PRIMARY KEY,
  Login_name nvarchar(8) not null,
  Login_pass nvarchar(8) not null,
);

CREATE TABLE Thema (
  Thema_id INT IDENTITY (1, 1) PRIMARY KEY, 
  Login_id int not null,
  Thema_Text ntext not null,
  constraint fk_LoginThema foreign key (Login_id) references Login(Login_id)
);

CREATE TABLE Stil (
  Stil_id INT IDENTITY (1, 1) PRIMARY KEY, 
  Thema_id int not null,
  Stil_Text ntext not null,
	CONSTRAINT fk_ThemaStil FOREIGN KEY (Thema_id) REFERENCES Thema(Thema_id)
);

CREATE TABLE Ausgabe (
  Ausgabe_id INT IDENTITY (1, 1) PRIMARY KEY, 
  Ausgabe_Text ntext not null,
  Stil_id int foreign key references Stil(Stil_id)

);
