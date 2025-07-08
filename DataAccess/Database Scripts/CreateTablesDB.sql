use [brightcodeProyectoII-db]
go

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    MobilePhone VARCHAR(15) NOT NULL UNIQUE,
    ProfilePhoto VARCHAR(300),
    IDPhotoFront VARCHAR(300),
    IDPhotoBack VARCHAR(300),
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    EmailVerified BIT DEFAULT 0,
    MobileVerified BIT DEFAULT 0,
    BiometricVerified BIT DEFAULT 0,
    UserStatus BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME
);

CREATE TABLE FinancialEntities (
    FinancialEntityID INT PRIMARY KEY IDENTITY(1,1),
    EntityName VARCHAR(100) NOT NULL,
    TaxID VARCHAR(20) NOT NULL UNIQUE,
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    ContactPhone VARCHAR(15) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    CommissionPercentage DECIMAL(5,2),
    ValidationStatus BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME
);

CREATE TABLE UserFinancialEntity (
    FinancialEntityID INT NOT NULL,
    AccessUsername VARCHAR(50) NOT NULL,
    AccessPassword VARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (AccessUsername, FinancialEntityID),
    FOREIGN KEY (FinancialEntityID) REFERENCES FinancialEntities(FinancialEntityID)
);

CREATE TABLE Merchants (
    MerchantID INT PRIMARY KEY IDENTITY(1,1),
    MerchantName VARCHAR(100) NOT NULL,
    TaxID VARCHAR(20) NOT NULL UNIQUE,
    LogoImage VARCHAR(300),
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    Phone VARCHAR(15) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    CommissionPercentage DECIMAL(5,2),
    ValidationStatus BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME
);

CREATE TABLE UserMerchant (
    MerchantID INT NOT NULL,
    AccessUsername VARCHAR(50) NOT NULL,
    AccessPassword VARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (AccessUsername, MerchantID),
    FOREIGN KEY (MerchantID) REFERENCES Merchants(MerchantID)
);

CREATE TABLE BankAccounts (
    BankAccountID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    IBAN CHAR(22) NOT NULL,
    FinancialEntityID INT NOT NULL,
    Status BIT DEFAULT 1,
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
    AdminStatus BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME
);

CREATE TABLE FinancialPromotions (
    PromotionID INT PRIMARY KEY IDENTITY(1,1),
    FinancialEntityID INT NOT NULL,
    PromotionType VARCHAR(20) NOT NULL CHECK (PromotionType IN ('Time', 'Quantity')),
    DiscountPercentage DECIMAL(5,2) NOT NULL,
    MaxRefund DECIMAL(12,2) NOT NULL,
    StartDate DATETIME,
    EndDate DATETIME,
    AvailableQuantity INT,
    Status BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FinancialEntityID) REFERENCES FinancialEntities(FinancialEntityID)
);

CREATE TABLE MerchantPromotions (
    PromotionID INT PRIMARY KEY IDENTITY(1,1),
    MerchantID INT NOT NULL,
    PromotionType VARCHAR(20) NOT NULL CHECK (PromotionType IN ('Time', 'Quantity')),
    DiscountPercentage DECIMAL(5,2) NOT NULL,
    MaxRefund DECIMAL(12,2) NOT NULL,
    StartDate DATETIME,
    EndDate DATETIME,
    AvailableQuantity INT,
    Status BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MerchantID) REFERENCES Merchants(MerchantID)
);
