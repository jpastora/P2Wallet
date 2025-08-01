    use [yavidb]
go

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
	IDNumber VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
	BirthDate DATETIME NOT NULL,
    MobilePhone VARCHAR(15) NOT NULL UNIQUE,
    ProfilePhoto VARCHAR(300),
    IDPhotoFront VARCHAR(300) NOT NULL,
    IDPhotoBack VARCHAR(300) NOT NULL,
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    EmailVerified VARCHAR(15) DEFAULT 'Inactive' CHECK (EmailVerified IN ('Active', 'Inactive')),
    MobileVerified VARCHAR(15) DEFAULT 'Inactive' CHECK (MobileVerified IN ('Active', 'Inactive')),
    BiometricVerified VARCHAR(15) DEFAULT 'Inactive' CHECK (BiometricVerified IN ('Active', 'Inactive')),
    ValidationStatus VARCHAR(15) DEFAULT 'Inactive' CHECK (ValidationStatus IN ('Active', 'Inactive')),
	SMSNotificaction VARCHAR(15) DEFAULT 'Inactive' CHECK (SMSNotificaction IN ('Active', 'Inactive')),
	PushNotification VARCHAR(15) DEFAULT 'Inactive' CHECK (PushNotification IN ('Active', 'Inactive')),
	EmailNotification VARCHAR(15) DEFAULT 'Inactive' CHECK (EmailNotification IN ('Active', 'Inactive')),
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME
);

CREATE TABLE FinancialEntities (
    FinancialEntityID INT PRIMARY KEY IDENTITY(1,1),
    EntityName VARCHAR(100) NOT NULL,
    TaxID VARCHAR(20) NOT NULL UNIQUE,
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    ContactPhone VARCHAR(15) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL,
    CommissionPercentage DECIMAL(5,2),
    ValidationStatus VARCHAR(15) DEFAULT 'Inactive' CHECK (ValidationStatus IN ('Active', 'Inactive')),
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME
);

CREATE TABLE UserFinancialEntity (
    FinancialEntityID INT NOT NULL,
    UserID INT NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserID, FinancialEntityID),
    FOREIGN KEY (FinancialEntityID) REFERENCES FinancialEntities(FinancialEntityID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE Merchants (
    MerchantID INT PRIMARY KEY IDENTITY(1,1),
    MerchantName VARCHAR(100) NOT NULL,
    TaxID VARCHAR(20) NOT NULL UNIQUE,
    LogoImage VARCHAR(300),
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    ContactPhone VARCHAR(15) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL,
    CommissionPercentage DECIMAL(5,2),
    ValidationStatus VARCHAR(15) DEFAULT 'Inactive' CHECK (ValidationStatus IN ('Active', 'Inactive')),
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME
);

CREATE TABLE UserFinancialEntity (
    FinancialEntityID INT NOT NULL,
    UserID INT NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserID, FinancialEntityID),
    FOREIGN KEY (FinancialEntityID) REFERENCES FinancialEntities(FinancialEntityID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE BankAccounts (
    BankAccountID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    IBAN CHAR(22) NOT NULL,
    FinancialEntityID INT NOT NULL,
    ValidationStatus VARCHAR(15) DEFAULT 'Active' CHECK (ValidationStatus IN ('Active', 'Inactive')),
    RegisteredAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (FinancialEntityID) REFERENCES FinancialEntities(FinancialEntityID)
);

CREATE TABLE Transactions (
    TransactionID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    MerchantID INT NOT NULL,
    BankAccountID INT NOT NULL,
    GrossAmount DECIMAL(12,2) NOT NULL,
    NetAmount DECIMAL(12,2) NOT NULL,
    DiscountApplied DECIMAL(12,2) NOT NULL,
    CommissionApplied DECIMAL(12,2) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    TransactionStatus VARCHAR(20) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (MerchantID) REFERENCES Merchants(MerchantID),
    FOREIGN KEY (BankAccountID) REFERENCES BankAccounts(BankAccountID)
);

CREATE TABLE Administrators (
    AdminID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(20) NOT NULL,
    AccessUsername VARCHAR(50) NOT NULL,
    AccessPassword VARCHAR(255) NOT NULL,
    ValidationStatus VARCHAR(15) DEFAULT 'Active' CHECK (ValidationStatus IN ('Active', 'Inactive')),
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME
);

CREATE TABLE FinancialPromotions (
    PromotionID INT PRIMARY KEY IDENTITY(1,1),
    FinancialEntityID INT NOT NULL,
    PromotionType VARCHAR(20) NOT NULL CHECK (PromotionType IN ('Time', 'Quantity')),
    DiscountPercentage DECIMAL(5,2) NOT NULL,
    MaxRefund DECIMAL(12,2) NOT NULL,
    StartDate DATETIME DEFAULT NULL,
    EndDate DATETIME DEFAULT NULL,
    AvailableQuantity INT DEFAULT NULL,
    ValidationStatus VARCHAR(15) DEFAULT 'Active' CHECK (ValidationStatus IN ('Active', 'Inactive')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FinancialEntityID) REFERENCES FinancialEntities(FinancialEntityID)
);

CREATE TABLE MerchantPromotions (
    PromotionID INT PRIMARY KEY IDENTITY(1,1),
    MerchantID INT NOT NULL,
    PromotionType VARCHAR(20) NOT NULL CHECK (PromotionType IN ('Time', 'Quantity')),
    DiscountPercentage DECIMAL(5,2) NOT NULL,
    MaxRefund DECIMAL(12,2) NOT NULL,
    StartDate DATETIME DEFAULT NULL,
    EndDate DATETIME DEFAULT NULL,
    AvailableQuantity INT DEFAULT NULL,
    ValidationStatus VARCHAR(15) DEFAULT 'Active' CHECK (ValidationStatus IN ('Active', 'Inactive')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MerchantID) REFERENCES Merchants(MerchantID)
);

CREATE TABLE AccountMovements
(
    MovementID INT IDENTITY(1,1) PRIMARY KEY,
    TransactionID INT NOT NULL,
    SourceAccountDescription VARCHAR(100) NOT NULL,
    DestinationAccountDescription VARCHAR(100) NOT NULL,
    MovementType VARCHAR(50) NOT NULL,
    Amount DECIMAL(12, 2) NOT NULL,
    Timestamp DATETIME NOT NULL,
	FOREIGN KEY (TransactionID) REFERENCES Transactions(TransactionID)
);

