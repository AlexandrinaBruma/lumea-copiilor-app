CREATE DATABASE Lumea_Copiilor;
USE Lumea_Copiilor;

CREATE TABLE Product(
	ProductID INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(50) NOT NULL,
	Min_age INT NOT NULL,
	Max_age INT,
	Fab_date DATETIME NOT NULL,
	Exp_date DATETIME,
	Price DECIMAL(10,2) NOT NULL,
	Origin_country INT NOT NULL,
	Importator INT NOT NULL,
	Shop INT NOT NULL, 
	Quantity INT NOT NULL,
	Category INT NOT NULL,

	CHECK(Max_age IS NULL OR Max_age > Min_age),
	CHECK(Exp_date IS NULL OR Exp_date > Fab_date),
	CHECK(Min_age >= 0),
	CHECK(Price > 0),
	CHECK(Quantity >= 0)
);

CREATE TABLE OutOfStock(
	OutOfStockID INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(50) NOT NULL,
	Min_age INT NOT NULL,
	Max_age INT,
	Fab_date DATETIME NOT NULL,
	Exp_date DATETIME,
	Price DECIMAL(10,2) NOT NULL,
	Origin_country INT NOT NULL,
	Importator INT NOT NULL,
	Shop INT NOT NULL, 
	Category INT NOT NULL,
	Archived_date DATETIME NOT NULL 
);

CREATE TABLE Shop(
	ShopID INT PRIMARY KEY IDENTITY(1,1),
	Street_address NVARCHAR(50) NOT NULL,
	Opening_date DATE NOT NULL,
	Closing_date DATE,
	Opening_hour TIME NOT NULL,
	Closing_hour TIME NOT NULL,
	Max_capacity INT NOT NULL,
	City INT NOT NULL,
	Phone NVARCHAR(20) NOT NULL,
	Email NVARCHAR(80) NOT NULL,
	Website NVARCHAR(80) DEFAULT 'lumeacopiilor.com',

	CHECK(Closing_date IS NULL OR Closing_date > Opening_date),
	CHECK(Closing_hour > Opening_hour),
	CHECK(Max_capacity > 0)
);

CREATE TABLE Utilizator(
	UtilizatorID INT PRIMARY KEY IDENTITY(1,1),
	Username NVARCHAR(80) NOT NULL,
	Passwd NVARCHAR(80) NOT NULL,
	Name NVARCHAR(50) NOT NULL,
	Surname NVARCHAR(50) NOT NULL,
	Birthdate DATE,
	Email NVARCHAR(80) UNIQUE,
	Phone_number NVARCHAR(20) UNIQUE,
	Gender CHAR(1) CHECK(Gender IN ('M', 'F')),
	Registration_date DATETIME NOT NULL,
	City INT NOT NULL,
	Role CHAR(1) CHECK(Role IN ('A', 'U')),

	CHECK(Birthdate < GETDATE()),
	CHECK(Registration_date <= GETDATE())
);

CREATE TABLE Purchase(
	PurchaseID INT PRIMARY KEY IDENTITY(1,1),
	Client INT NOT NULL,
	Shop INT NOT NULL,
	Product INT NOT NULL,
	Payment_type INT NOT NULL,
	Purchase_date DATETIME NOT NULL,
	Quantity INT NOT NULL,
	Card_number NVARCHAR(20),

	CHECK(Quantity > 0),
	CHECK(Purchase_date <= GETDATE())
);

CREATE TABLE Importator(
	ImportatorID INT PRIMARY KEY IDENTITY(1,1),
	Company_name NVARCHAR(50) NOT NULL,
	Contact_person NVARCHAR(50) NOT NULL,
	Phone_number NVARCHAR(20) UNIQUE,
	Email NVARCHAR(80) UNIQUE,
	Website NVARCHAR(50) UNIQUE,
	Street_address NVARCHAR(50) NOT NULL,
	City INT NOT NULL,
	Fiscal_code NVARCHAR(50) NOT NULL,
	Contract_start_date DATE NOT NULL,
	Contract_end_date DATE,

	CHECK(Contract_end_date IS NULL OR Contract_end_date > Contract_start_date),
	CHECK(LEN(Fiscal_code) >= 6)
);

CREATE TABLE Payment(
	PaymentID INT PRIMARY KEY IDENTITY(1,1),
	Type NVARCHAR(50) NOT NULL
);

CREATE TABLE Country(
	CountryID INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(50) NOT NULL
);

CREATE TABLE City(
	CityID INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(50) NOT NULL,
	Country INT NOT NULL
);

CREATE TABLE Category(
	CategoryID INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(50) NOT NULL
);	

-- FOREIGN KEYS:

-- Tables Product:
ALTER TABLE Product
ADD FOREIGN KEY (Origin_country) REFERENCES Country(CountryID);

ALTER TABLE Product
ADD FOREIGN KEY (Importator) REFERENCES Importator(ImportatorID);

ALTER TABLE Product
ADD FOREIGN KEY (Shop) REFERENCES Shop(ShopID);

ALTER TABLE Product
ADD FOREIGN KEY (Category) REFERENCES Category(CategoryID);

-- Table OutOfStock:
ALTER TABLE OutOfStock
ADD FOREIGN KEY (Origin_country) REFERENCES Country(CountryID);

ALTER TABLE OutOfStock
ADD FOREIGN KEY (Importator) REFERENCES Importator(ImportatorID);

ALTER TABLE OutOfStock
ADD FOREIGN KEY (Shop) REFERENCES Shop(ShopID);

ALTER TABLE OutOfStock
ADD FOREIGN KEY (Category) REFERENCES Category(CategoryID);

-- Table City:
ALTER TABLE City
ADD FOREIGN KEY (Country) REFERENCES Country(CountryID);

-- Table Shop:
ALTER TABLE Shop
ADD FOREIGN KEY (City) REFERENCES City(CityID);

-- Table Purchase:
ALTER TABLE Purchase
ADD FOREIGN KEY (Client) REFERENCES Utilizator(UtilizatorID);

ALTER TABLE Purchase
ADD FOREIGN KEY (Shop) REFERENCES Shop(ShopID);

ALTER TABLE Purchase
ADD FOREIGN KEY (Product) REFERENCES Product(ProductID);

ALTER TABLE Purchase
ADD FOREIGN KEY (Payment_type) REFERENCES Payment(PaymentID);

-- Table Importator:
ALTER TABLE Importator
ADD FOREIGN KEY (City) REFERENCES City(CityID);

-- INSERTING DATA INTO TABLES:

INSERT INTO Country (Name) VALUES
('Romania'),
('Germania'),
('China'),
('Franta'),
('Polonia'),
('Turcia'),
('Italia'),
('Spania'),
('Cehia'),
('Ungaria'),
('Moldova');

INSERT INTO City (Name, Country) VALUES
('Chisinau', 11), 
('Bucuresti', 1),
('Cluj-Napoca', 1),
('Iasi', 1),
('Berlin', 2),
('Shanghai', 3),
('Paris', 4),
('Varsovia', 5),
('Istanbul', 6),
('Milano', 7);

INSERT INTO Shop (Street_address, Opening_date, Opening_hour, Closing_hour, Max_capacity, City, Phone, Email) VALUES
('Str. Stefan cel Mare 10',  '2015-03-01', '09:00', '20:00', 100, 1, '+37322123456', 'chisinau@lumeacopiilor.com'),
('Bd. Unirii 45',            '2016-06-15', '09:00', '21:00', 150, 2, '+40721111111', 'bucuresti@lumeacopiilor.com'),
('Str. Memorandumului 7',    '2017-09-20', '10:00', '20:00', 80,  3, '+40744222222', 'cluj@lumeacopiilor.com'),
('Str. Lapusneanu 3',        '2018-01-10', '09:30', '20:30', 90,  4, '+40755333333', 'iasi@lumeacopiilor.com'),
('Str. Puskin 22',           '2019-04-05', '10:00', '19:00', 70,  1, '+37322654321', 'centru@lumeacopiilor.com'),
('Bd. Mircea cel Batran 88', '2020-02-14', '09:00', '20:00', 120, 1, '+37322789012', 'botanica@lumeacopiilor.com'),
('Str. Armeneasca 5',        '2021-07-01', '10:00', '19:30', 60,  1, '+37322345678', 'armeneasca@lumeacopiilor.com'),
('Calea Mostilor 100',       '2014-11-11', '08:30', '21:30', 200, 2, '+40766444444', 'mosilor@lumeacopiilor.com'),
('Str. Horea 2',             '2022-03-03', '09:00', '20:00', 75,  3, '+40777555555', 'horea@lumeacopiilor.com'),
('Str. Arcu 15',             '2023-05-20', '10:00', '19:00', 65,  4, '+40788666666', 'arcu@lumeacopiilor.com');
 
INSERT INTO Importator (Company_name, Contact_person, Phone_number, Email, Website, Street_address, City, Fiscal_code, Contract_start_date) VALUES
('ToyWorld SRL',    'Andrei Popescu', '+37322100001', 'contact@toyworld.md',  'www.toyworld.md',      'Str. Industriala 1',  1, 'MD1234567', '2015-01-01'),
('KidImport SRL',   'Maria Ionescu',  '+40721200002', 'office@kidimport.ro',  'www.kidimport.ro',     'Bd. Industriilor 20', 2, 'RO2345678', '2016-03-15'),
('EuroToys SA',     'Luca Brambilla', '+39025300003', 'info@eurotoys.it',     'www.eurotoys.it',      'Via Roma 5',          10,'IT3456789', '2017-06-01'),
('AsiaKids SRL',    'Wei Zhang',      '+86215400004', 'sales@asiakids.cn',    'www.asiakids.cn',      'Nanjing Road 88',     6, 'CN4567890', '2018-02-10'),
('PlayZone GmbH',   'Hans Muller',    '+49305500005', 'hello@playzone.de',    'www.playzone.de',      'Hauptstrasse 12',     5, 'DE5678901', '2019-09-01'),
('FunToys SARL',    'Sophie Dubois',  '+33156600006', 'bonjour@funtoys.fr',   'www.funtoys.fr',       'Rue de la Paix 3',    7, 'FR6789012', '2020-04-20'),
('PolandPlay SP',   'Anna Kowalski',  '+48227700007', 'biuro@polandplay.pl',  'www.polandplay.pl',    'Ul. Nowy Swiat 10',   8, 'PL7890123', '2021-01-15'),
('IstanbulToy AS',  'Mehmet Yilmaz',  '+90212800008', 'info@istanbultoy.com', 'www.istanbultoy.com',  'Istiklal Caddesi 7',  9, 'TR8901234', '2021-08-01'),
('CzechFun SRO',    'Jan Novak',      '+420229900009','info@czechfun.cz',     'www.czechfun.cz',      'Vaclavske Namesti 1', 1, 'CZ9012345', '2022-03-10'),
('HungaryKids KFT', 'Peter Szabo',    '+361230000010','hello@hungarykids.hu', 'www.hungarykids.hu',   'Andrassy Ut 22',      1, 'HU0123456', '2023-01-05');

 INSERT INTO Category (Name) VALUES
('Jucarii educative'),
('Jocuri de societate'),
('Figurine si papusi'),
('Jucarii de exterior'),
('Constructii si Lego'),
('Jucarii bebelusi'),
('Vehicule si masinute'),
('Arte si creativitate'),
('Jocuri electronice'),
('Carti si puzzle');
 
INSERT INTO Payment (Type) VALUES
('Numerar'),
('Card bancar'),
('Apple Pay'),
('Google Pay'),
('PayPal');

INSERT INTO Product (Name, Min_age, Fab_date, Price, Origin_country, Importator, Shop, Quantity, Category) VALUES
('Set Lego City 500 piese',          5,  '2023-01-10', 249.99, 2, 5, 1,  30, 5),
('Papusa Barbie Deluxe',             3,  '2023-03-15', 149.99, 3, 4, 2,  50, 3),
('Tricicleta colorata',              2,  '2022-11-20', 399.99, 7, 3, 3,  15, 4),
('Puzzle 1000 piese Natura',         8,  '2023-05-01', 89.99,  4, 6, 4,  40, 10),
('Joc Monopoly Clasic',              8,  '2023-02-28', 129.99, 2, 5, 1,  25, 2),
('Cuburi moi bebelusi',              0,  '2023-06-10', 49.99,  3, 4, 5,  60, 6),
('Masinuta telecomanda',             6,  '2022-09-05', 199.99, 3, 4, 6,  20, 7),
('Set pictura acuarela',             4,  '2023-04-20', 69.99,  4, 6, 7,  35, 8),
('Consola portabila mini',           7,  '2023-07-15', 349.99, 3, 4, 8,  18, 9),
('Figurina Spiderman',               4,  '2023-01-25', 79.99,  3, 4, 9,  45, 3),
('Set stiinte chimie junior',        10, '2023-03-01', 159.99, 2, 5, 10, 22, 1),
('Leagan exterior lemn',             3,  '2022-08-15', 599.99, 7, 3, 1,  8,  4),
('Joc Scrabble Romanian',            8,  '2023-02-10', 99.99,  1, 3, 2,  30, 2),
('Set constructie magnetic',         4,  '2023-05-20', 179.99, 3, 4, 3,  27, 5),
('Bicicleta 16 inch rosie',          5,  '2023-06-01', 699.99, 2, 5, 4,  12, 4),
('Set Lego Technic 800 piese',       9,  '2023-02-01', 399.99, 2, 5, 1,  20, 5),
('Papusa LOL Surprise',              3,  '2023-04-10', 119.99, 3, 4, 2,  40, 3),
('Trotineta copii 3 roti',           3,  '2023-01-20', 299.99, 5, 7, 3,  18, 4),
('Puzzle 500 piese Orase',           6,  '2023-03-15', 74.99,  4, 6, 4,  35, 10),
('Joc Cluedo',                       8,  '2023-05-10', 139.99, 2, 5, 5,  22, 2),
('Jucarie zornaitoare bebe',         0,  '2023-06-20', 29.99,  3, 4, 6,  80, 6),
('Camion de pompieri RC',            5,  '2023-02-15', 249.99, 3, 4, 7,  15, 7),
('Set desen pasteluri 36',           4,  '2023-04-05', 54.99,  4, 6, 8,  50, 8),
('Tableta educativa copii',          4,  '2023-07-01', 449.99, 3, 4, 9,  14, 9),
('Figurina Batman',                  4,  '2023-02-20', 84.99,  3, 4, 10, 38, 3),
('Microscop junior set',             8,  '2023-03-20', 189.99, 2, 5, 1,  16, 1),
('Tobogan plastic mini',             2,  '2023-01-05', 449.99, 7, 3, 2,  6,  4),
('Joc Rummy clasic',                 7,  '2023-02-25', 89.99,  9, 8, 3,  28, 2),
('Set cuburi lemn 50 piese',         1,  '2023-05-15', 99.99,  1, 1, 4,  32, 5),
('Bicicleta 20 inch albastra',       7,  '2023-06-15', 799.99, 2, 5, 5,  10, 4),
('Set Lego Friends 300 piese',       6,  '2023-03-10', 199.99, 2, 5, 6,  25, 5),
('Casa papusilor',                   4,  '2023-04-25', 329.99, 3, 4, 7,  12, 3),
('Trotineta electrica junior',       8,  '2023-05-30', 899.99, 3, 4, 8,  8,  4),
('Puzzle 3D Turnul Eiffel',          10, '2023-06-05', 109.99, 4, 6, 9,  20, 10),
('Joc Dobble',                       4,  '2023-01-15', 79.99,  4, 6, 10, 30, 2),
('Jucarie baie set 5 piese',         1,  '2023-07-10', 39.99,  3, 4, 1,  55, 6),
('Excavator die-cast metal',         3,  '2023-02-05', 89.99,  3, 4, 2,  42, 7),
('Set origami 200 coli',             6,  '2023-03-25', 44.99,  3, 4, 3,  48, 8),
('Robot interactiv programabil',     7,  '2023-07-20', 499.99, 3, 4, 4,  11, 9),
('Figurina Wonder Woman',            4,  '2023-04-15', 74.99,  3, 4, 5,  40, 3),
('Telescop copii 50x',               8,  '2023-05-05', 219.99, 2, 5, 6,  14, 1),
('Piscina gonflabila mare',          2,  '2023-05-25', 349.99, 6, 8, 7,  9,  4),
('Joc Pictionary junior',            7,  '2023-02-10', 114.99, 4, 6, 8,  26, 2),
('Set Lego Duplo 80 piese',          2,  '2023-06-25', 149.99, 2, 5, 9,  28, 5),
('Bicicleta 12 inch galbena',        3,  '2023-04-30', 549.99, 2, 5, 10, 14, 4),
('Papusa bebelus cu accesorii',      2,  '2023-03-05', 169.99, 3, 4, 1,  22, 3),
('Masinuta Ferrari die-cast',        3,  '2023-01-30', 59.99,  3, 4, 2,  55, 7),
('Set modelare lut 10 culori',       4,  '2023-04-01', 64.99,  5, 7, 3,  44, 8),
('Drona mini pentru copii',          10, '2023-07-25', 299.99, 3, 4, 4,  12, 9),
('Figurina Iron Man',                4,  '2023-02-28', 89.99,  3, 4, 5,  36, 3),
('Set experimente fizica',           9,  '2023-03-12', 174.99, 2, 5, 6,  18, 1),
('Trambulina exterior 3m',           4,  '2023-05-08', 1299.99,7, 3, 7,  5,  4),
('Joc Alias junior',                 7,  '2023-02-18', 94.99,  5, 7, 8,  24, 2),
('Set Lego Star Wars 600 piese',     9,  '2023-06-12', 349.99, 2, 5, 9,  16, 5),
('Bicicleta 24 inch verde',          10, '2023-07-05', 999.99, 2, 5, 10, 8,  4),
('Papusa Frozen Elsa',               3,  '2023-03-22', 129.99, 3, 4, 1,  35, 3),
('Ambulanta RC cu sunete',           4,  '2023-02-22', 179.99, 3, 4, 2,  28, 7),
('Creion 3D pentru copii',           8,  '2023-04-18', 189.99, 3, 4, 3,  20, 8),
('Consola jocuri 64GB',              7,  '2023-07-28', 599.99, 3, 4, 4,  10, 9),
('Figurina Thor',                    4,  '2023-01-28', 84.99,  3, 4, 5,  42, 3),
('Kit robotica educativa',           10, '2023-05-15', 299.99, 2, 5, 6,  12, 1),
('Casuta de joaca exterior',         3,  '2023-06-02', 1499.99,7, 3, 7,  4,  4),
('Joc Twister',                      6,  '2023-03-08', 69.99,  2, 5, 8,  32, 2),
('Set Lego Architecture',            12, '2023-07-08', 279.99, 2, 5, 9,  18, 5),
('Bicicleta 14 inch roz',            4,  '2023-04-22', 649.99, 2, 5, 10, 11, 4),
('Papusa Rapunzel deluxe',           3,  '2023-05-12', 139.99, 3, 4, 1,  30, 3),
('Tractor agricol RC',               5,  '2023-02-08', 219.99, 3, 4, 2,  22, 7),
('Set caligrafie si acuarela',       6,  '2023-04-28', 79.99,  4, 6, 3,  38, 8),
('Ochelari VR copii',                10, '2023-08-01', 399.99, 3, 4, 4,  9,  9),
('Figurina Hulk',                    4,  '2023-03-08', 79.99,  3, 4, 5,  44, 3),
('Planetariu de buzunar',            8,  '2023-04-08', 144.99, 2, 5, 6,  16, 1),
('Loc de joaca modular exterior',    3,  '2023-06-18', 1999.99,7, 3, 7,  3,  4),
('Joc Jenga clasic lemn',            6,  '2023-03-18', 74.99,  1, 1, 8,  36, 2),
('Set Lego Harry Potter',            8,  '2023-07-15', 319.99, 2, 5, 9,  14, 5),
('Bicicleta BMX copii',              8,  '2023-05-18', 849.99, 2, 5, 10, 9,  4),
('Papusa Monster High',              6,  '2023-03-28', 109.99, 3, 4, 1,  28, 3),
('Masina de politie cu telecomanda', 4,  '2023-02-12', 159.99, 3, 4, 2,  32, 7),
('Set stampile si culori',           3,  '2023-04-12', 49.99,  5, 7, 3,  52, 8),
('Joc educativ matematica',          5,  '2023-05-22', 119.99, 2, 5, 4,  24, 9),
('Figurina Capitanul America',       4,  '2023-03-18', 79.99,  3, 4, 5,  40, 3),
('Set chimie experimentala',         12, '2023-06-08', 249.99, 2, 5, 6,  10, 1),
('Leagan cu bara exterior',          4,  '2023-07-12', 699.99, 7, 3, 7,  6,  4),
('Joc Risk strategic',               10, '2023-02-05', 159.99, 2, 5, 8,  20, 2),
('Set Lego Minecraft',               8,  '2023-07-22', 289.99, 2, 5, 9,  16, 5),
('Tricicleta electrica copii',       3,  '2023-06-28', 1099.99,3, 4, 10, 7,  4),
('Papusa Ken cu accesorii',          3,  '2023-04-08', 99.99,  3, 4, 1,  34, 3),
('Buldozer RC mare',                 5,  '2023-03-02', 229.99, 3, 4, 2,  18, 7),
('Set vopsele acrilice 24',          6,  '2023-05-02', 84.99,  4, 6, 3,  42, 8),
('Camera foto instant copii',        6,  '2023-08-05', 279.99, 3, 4, 4,  16, 9),
('Figurina Thanos',                  8,  '2023-04-22', 99.99,  3, 4, 5,  30, 3),
('Set astronomie copii',             8,  '2023-05-28', 199.99, 2, 5, 6,  14, 1),
('Piscina cu bile 200 bile',         1,  '2023-06-22', 249.99, 5, 7, 7,  10, 4),
('Joc Uno deluxe',                   5,  '2023-01-22', 54.99,  3, 4, 8,  50, 2),
('Set Lego Classic 1000 piese',      5,  '2023-07-30', 229.99, 2, 5, 9,  20, 5),
('Bicicleta 18 inch portocalie',     6,  '2023-05-28', 749.99, 2, 5, 10, 10, 4),
('Papusa Moana Disney',              3,  '2023-04-18', 119.99, 3, 4, 1,  32, 3),
('Elicopter RC cu giroscop',         8,  '2023-03-08', 269.99, 3, 4, 2,  15, 7),
('Set creatie bijuterii copii',      6,  '2023-05-08', 74.99,  5, 7, 3,  46, 8),
('Smartwatch copii GPS',             5,  '2023-08-10', 349.99, 3, 4, 4,  18, 9),
('Figurina Aquaman',                 4,  '2023-03-25', 79.99,  3, 4, 5,  38, 3),
('Set biologie plante',              9,  '2023-06-15', 169.99, 2, 5, 6,  16, 1),
('Topogan exterior mare',            3,  '2023-07-18', 799.99, 7, 3, 7,  5,  4),
('Joc Saboteur carduri',             8,  '2023-02-28', 84.99,  4, 6, 8,  28, 2),
('Set Lego Speed Champions',         7,  '2023-08-02', 259.99, 2, 5, 9,  18, 5),
('Trotinetaa 2 roti copii',          5,  '2023-06-10', 359.99, 5, 7, 10, 13, 4),
('Papusa Stea Disney',               3,  '2023-04-28', 109.99, 3, 4, 1,  36, 3),
('Macara telescopica RC',            6,  '2023-03-15', 199.99, 3, 4, 2,  20, 7),
('Set broderie creativa',            7,  '2023-05-15', 69.99,  5, 7, 3,  40, 8),
('Laptop educational copii',         4,  '2023-08-15', 499.99, 3, 4, 4,  14, 9),
('Figurina Flash',                   4,  '2023-04-02', 74.99,  3, 4, 5,  42, 3),
('Set vulcan chimic',                10, '2023-07-02', 139.99, 2, 5, 6,  18, 1),
('Scaun balansoar copii exterior',   2,  '2023-08-08', 499.99, 7, 3, 7,  7,  4),
('Joc Taboo junior',                 8,  '2023-03-05', 104.99, 4, 6, 8,  24, 2),
('Set Lego Creator 3 in 1',          7,  '2023-08-18', 239.99, 2, 5, 9,  16, 5),
('Bicicleta 26 inch neagra',         12, '2023-07-28', 1199.99,2, 5, 10, 7,  4),
('Papusa Tinker Bell',               3,  '2023-05-05', 99.99,  3, 4, 1,  38, 3),
('Autobuz scolar RC',                4,  '2023-03-22', 169.99, 3, 4, 2,  25, 7),
('Set mozaic creativ',               5,  '2023-05-18', 59.99,  5, 7, 3,  48, 8),
('Ochelari AR educationali',         10, '2023-08-20', 549.99, 3, 4, 4,  8,  9),
('Figurina Green Lantern',           4,  '2023-04-05', 74.99,  3, 4, 5,  36, 3),
('Kit electricitate pentru copii',   9,  '2023-07-08', 179.99, 2, 5, 6,  14, 1),
('Arc cu sageti spuma',              6,  '2023-06-25', 89.99,  5, 7, 7,  30, 4),
('Joc Dixit carduri ilustrate',      8,  '2023-03-12', 119.99, 4, 6, 8,  22, 2),
('Set Lego Ninjago 700 piese',       9,  '2023-08-22', 299.99, 2, 5, 9,  14, 5),
('Papusa Ariel Mica Sirena',         3,  '2023-05-15', 124.99, 3, 4, 1,  32, 3),
('Masina sport RC drift',            6,  '2023-03-28', 239.99, 3, 4, 2,  17, 7),
('Set design moda copii',            6,  '2023-05-25', 89.99,  5, 7, 3,  36, 8),
('Ceas desteptator copii',           5,  '2023-08-25', 149.99, 3, 4, 4,  22, 9),
('Figurina Cyborg',                  4,  '2023-04-12', 74.99,  3, 4, 5,  38, 3);

INSERT INTO Utilizator (Username, Passwd, Name, Surname, Email, Phone_number, Registration_date, City, Role) VALUES
('ion.moraru',         'Pass@1234',  'Ion',        'Moraru',     'ion.moraru@gmail.com',              '+37360100001', '2022-01-15 10:30:00', 1, 'U'),
('maria.dumbrava',     'Pass@1234',  'Maria',      'Dumbrava',   'maria.dumbrava@gmail.com',          '+37360100002', '2022-03-20 14:00:00', 2, 'U'),
('alexandru.popa',     'Pass@1234',  'Alexandru',  'Popa',       'alexandru.popa@gmail.com',          '+37360100003', '2022-06-05 09:15:00', 3, 'U'),
('elena.ciobanu',      'Pass@1234',  'Elena',      'Ciobanu',    'elena.ciobanu@gmail.com',           '+37360100004', '2022-08-11 16:45:00', 4, 'U'),
('vasile.rusu',        'Pass@1234',  'Vasile',     'Rusu',       'vasile.rusu@gmail.com',             '+37360100005', '2023-01-03 11:00:00', 1, 'U'),
('natalia.grama',      'Pass@1234',  'Natalia',    'Grama',      'natalia.grama@gmail.com',           '+37360100006', '2023-04-18 13:30:00', 1, 'U'),
('dumitru.lungu',      'Pass@1234',  'Dumitru',    'Lungu',      'dumitru.lungu@gmail.com',           '+37360100007', '2023-07-22 10:00:00', 2, 'U'),
('cristina.balan',     'Pass@1234',  'Cristina',   'Balan',      'cristina.balan@gmail.com',          '+37360100008', '2023-09-10 15:20:00', 3, 'U'),
('mihai.stefan',       'Admin@1234', 'Mihai',      'Stefan',     'mihai.stefan@lumeacopiilor.com',    '+37360100009', '2021-05-01 08:00:00', 1, 'A'),
('ana.vrabie',         'Admin@1234', 'Ana',        'Vrabie',     'ana.vrabie@lumeacopiilor.com',      '+37360100010', '2021-11-15 09:00:00', 1, 'A'),
('andrei.ionescu',     'Pass@1234',  'Andrei',     'Ionescu',    'andrei.ionescu@gmail.com',          '+37360100011', '2022-02-10 08:00:00', 1, 'U'),
('laura.stan',         'Pass@1234',  'Laura',      'Stan',       'laura.stan@gmail.com',              '+37360100012', '2022-04-25 10:30:00', 2, 'U'),
('george.marin',       'Pass@1234',  'George',     'Marin',      'george.marin@gmail.com',            '+37360100013', '2022-05-14 14:00:00', 3, 'U'),
('ioana.niculescu',    'Pass@1234',  'Ioana',      'Niculescu',  'ioana.niculescu@gmail.com',         '+37360100014', '2022-07-01 09:00:00', 4, 'U'),
('radu.gheorghe',      'Pass@1234',  'Radu',       'Gheorghe',   'radu.gheorghe@gmail.com',           '+37360100015', '2022-09-18 11:45:00', 1, 'U'),
('simona.apostol',     'Pass@1234',  'Simona',     'Apostol',    'simona.apostol@gmail.com',          '+37360100016', '2022-10-05 13:00:00', 2, 'U'),
('bogdan.florea',      'Pass@1234',  'Bogdan',     'Florea',     'bogdan.florea@gmail.com',           '+37360100017', '2022-11-20 15:30:00', 1, 'U'),
('oana.pop',           'Pass@1234',  'Oana',       'Pop',        'oana.pop@gmail.com',                '+37360100018', '2022-12-12 10:00:00', 3, 'U'),
('stefan.mihalcea',    'Pass@1234',  'Stefan',     'Mihalcea',   'stefan.mihalcea@gmail.com',         '+37360100019', '2023-01-28 08:30:00', 4, 'U'),
('diana.constantin',   'Pass@1234',  'Diana',      'Constantin', 'diana.constantin@gmail.com',        '+37360100020', '2023-02-14 12:00:00', 1, 'U'),
('iulian.barbu',       'Pass@1234',  'Iulian',     'Barbu',      'iulian.barbu@gmail.com',            '+37360100021', '2023-03-03 09:45:00', 2, 'U'),
('raluca.dragomir',    'Pass@1234',  'Raluca',     'Dragomir',   'raluca.dragomir@gmail.com',         '+37360100022', '2023-03-25 14:30:00', 1, 'U'),
('cosmin.zamfir',      'Pass@1234',  'Cosmin',     'Zamfir',     'cosmin.zamfir@gmail.com',           '+37360100023', '2023-04-10 11:00:00', 3, 'U'),
('gabriela.roman',     'Pass@1234',  'Gabriela',   'Roman',      'gabriela.roman@gmail.com',          '+37360100024', '2023-05-01 16:00:00', 4, 'U'),
('vlad.petrescu',      'Pass@1234',  'Vlad',       'Petrescu',   'vlad.petrescu@gmail.com',           '+37360100025', '2023-05-20 10:15:00', 1, 'U'),
('alina.tudose',       'Pass@1234',  'Alina',      'Tudose',     'alina.tudose@gmail.com',            '+37360100026', '2023-06-08 13:45:00', 2, 'U'),
('marius.enache',      'Pass@1234',  'Marius',     'Enache',     'marius.enache@gmail.com',           '+37360100027', '2023-06-22 09:00:00', 1, 'U'),
('teodora.neagu',      'Pass@1234',  'Teodora',    'Neagu',      'teodora.neagu@gmail.com',           '+37360100028', '2023-07-05 14:00:00', 3, 'U'),
('sorin.matei',        'Pass@1234',  'Sorin',      'Matei',      'sorin.matei@gmail.com',             '+37360100029', '2023-07-18 11:30:00', 4, 'U'),
('roxana.dinu',        'Pass@1234',  'Roxana',     'Dinu',       'roxana.dinu@gmail.com',             '+37360100030', '2023-08-02 08:00:00', 1, 'U'),
('catalin.stoica',     'Pass@1234',  'Catalin',    'Stoica',     'catalin.stoica@gmail.com',          '+37360100031', '2023-08-15 10:00:00', 2, 'U'),
('mihaela.olteanu',    'Pass@1234',  'Mihaela',    'Olteanu',    'mihaela.olteanu@gmail.com',         '+37360100032', '2023-08-28 15:00:00', 1, 'U'),
('florin.moldovan',    'Pass@1234',  'Florin',     'Moldovan',   'florin.moldovan@gmail.com',         '+37360100033', '2023-09-10 09:30:00', 3, 'U'),
('adriana.coman',      'Pass@1234',  'Adriana',    'Coman',      'adriana.coman@gmail.com',           '+37360100034', '2023-09-25 13:00:00', 4, 'U'),
('octavian.dobre',     'Pass@1234',  'Octavian',   'Dobre',      'octavian.dobre@gmail.com',          '+37360100035', '2023-10-08 11:00:00', 1, 'U'),
('luminita.toma',      'Pass@1234',  'Luminita',   'Toma',       'luminita.toma@gmail.com',           '+37360100036', '2023-10-20 14:30:00', 2, 'U'),
('dan.sirbu',          'Pass@1234',  'Dan',        'Sirbu',      'dan.sirbu@gmail.com',               '+37360100037', '2023-11-03 08:45:00', 1, 'U'),
('paula.avram',        'Pass@1234',  'Paula',      'Avram',      'paula.avram@gmail.com',             '+37360100038', '2023-11-15 12:00:00', 3, 'U'),
('liviu.nistor',       'Pass@1234',  'Liviu',      'Nistor',     'liviu.nistor@gmail.com',            '+37360100039', '2023-11-28 16:00:00', 4, 'U'),
('carmen.badea',       'Pass@1234',  'Carmen',     'Badea',      'carmen.badea@gmail.com',            '+37360100040', '2023-12-05 09:00:00', 1, 'U'),
('horia.popa',         'Pass@1234',  'Horia',      'Popa',       'horia.popa@gmail.com',              '+37360100041', '2023-12-12 11:30:00', 2, 'U'),
('veronica.antal',     'Pass@1234',  'Veronica',   'Antal',      'veronica.antal@gmail.com',          '+37360100042', '2024-01-05 10:00:00', 1, 'U'),
('sebastian.luca',     'Pass@1234',  'Sebastian',  'Luca',       'sebastian.luca@gmail.com',          '+37360100043', '2024-01-18 13:00:00', 3, 'U'),
('nicoleta.petre',     'Pass@1234',  'Nicoleta',   'Petre',      'nicoleta.petre@gmail.com',          '+37360100044', '2024-01-30 15:00:00', 4, 'U'),
('razvan.cirstea',     'Pass@1234',  'Razvan',     'Cirstea',    'razvan.cirstea@gmail.com',          '+37360100045', '2024-02-10 08:30:00', 1, 'U'),
('anca.serban',        'Pass@1234',  'Anca',       'Serban',     'anca.serban@gmail.com',             '+37360100046', '2024-02-20 10:45:00', 2, 'U'),
('ionut.chiriac',      'Pass@1234',  'Ionut',      'Chiriac',    'ionut.chiriac@gmail.com',           '+37360100047', '2024-03-01 09:00:00', 1, 'U'),
('elena.manea',        'Pass@1234',  'Elena',      'Manea',      'elena.manea@gmail.com',             '+37360100048', '2024-03-12 14:00:00', 3, 'U');

INSERT INTO Purchase (Client, Shop, Product, Payment_type, Purchase_date, Quantity) VALUES
(1,  1,  1,   1, '2024-01-05 10:15:00', 1),
(2,  2,  2,   2, '2024-01-10 14:30:00', 2),
(3,  3,  3,   2, '2024-01-12 11:00:00', 1),
(4,  4,  4,   1, '2024-01-15 16:00:00', 3),
(5,  1,  5,   5, '2024-01-18 09:30:00', 1),
(6,  5,  6,   2, '2024-01-20 13:00:00', 2),
(7,  6,  7,   2, '2024-01-22 15:45:00', 1),
(8,  7,  8,   1, '2024-01-25 10:00:00', 2),
(9,  8,  9,   2, '2024-01-28 17:00:00', 1),
(10, 9,  10,  1, '2024-02-01 11:30:00', 1),
(1,  10, 11,  2, '2024-02-03 14:00:00', 1),
(4,  1,  12,  1, '2024-02-05 09:00:00', 1),
(5,  2,  13,  5, '2024-02-07 16:30:00', 2),
(6,  3,  14,  2, '2024-02-10 12:00:00', 1),
(7,  4,  15,  2, '2024-02-12 10:45:00', 1),
(11, 1,  16,  1, '2024-02-14 09:30:00', 1),
(12, 2,  17,  2, '2024-02-15 14:00:00', 2),
(13, 3,  18,  2, '2024-02-16 11:30:00', 1),
(14, 4,  19,  1, '2024-02-17 16:00:00', 1),
(15, 5,  20,  3, '2024-02-18 10:00:00', 2),
(16, 6,  21,  2, '2024-02-19 13:30:00', 3),
(17, 7,  22,  1, '2024-02-20 15:00:00', 1),
(18, 8,  23,  4, '2024-02-21 09:00:00', 2),
(19, 9,  24,  2, '2024-02-22 11:00:00', 1),
(20, 10, 25,  1, '2024-02-23 14:30:00', 1),
(21, 1,  26,  2, '2024-02-24 10:15:00', 1),
(22, 2,  27,  1, '2024-02-25 13:00:00', 1),
(23, 3,  28,  5, '2024-02-26 16:00:00', 2),
(24, 4,  29,  2, '2024-02-27 09:30:00', 1),
(25, 5,  30,  1, '2024-02-28 11:30:00', 1),
(26, 6,  31,  3, '2024-03-01 14:00:00', 2),
(27, 7,  32,  2, '2024-03-02 10:00:00', 1),
(28, 8,  33,  1, '2024-03-03 15:30:00', 1),
(29, 9,  34,  4, '2024-03-04 09:00:00', 2),
(30, 10, 35,  2, '2024-03-05 12:00:00', 1),
(31, 1,  36,  1, '2024-03-06 10:30:00', 3),
(32, 2,  37,  2, '2024-03-07 14:00:00', 1),
(33, 3,  38,  3, '2024-03-08 11:00:00', 2),
(34, 4,  39,  1, '2024-03-09 16:00:00', 1),
(35, 5,  40,  2, '2024-03-10 09:30:00', 1),
(36, 6,  41,  4, '2024-03-11 13:00:00', 1),
(37, 7,  42,  1, '2024-03-12 15:00:00', 2),
(38, 8,  43,  2, '2024-03-13 10:00:00', 1),
(39, 9,  44,  5, '2024-03-14 14:30:00', 1),
(40, 10, 45,  1, '2024-03-15 11:00:00', 2),
(41, 1,  46,  2, '2024-03-16 09:00:00', 1),
(42, 2,  47,  3, '2024-03-17 13:30:00', 1),
(43, 3,  48,  1, '2024-03-18 16:00:00', 2),
(44, 4,  49,  2, '2024-03-19 10:30:00', 1),
(45, 5,  50,  4, '2024-03-20 12:00:00', 1);

INSERT INTO OutOfStock (Name, Min_age, Fab_date, Price, Origin_country, Importator, Shop, Category, Archived_date) VALUES
('Trenulet electric vintage',      4,  '2020-05-10', 299.99,  2, 5, 1,  7,  '2023-06-01 08:00:00'),
('Set bucatarie jucarie',          3,  '2019-11-20', 149.99,  3, 4, 2,  3,  '2023-07-15 09:00:00'),
('Tobogan plastic exterior',       2,  '2021-03-15', 499.99,  7, 3, 3,  4,  '2023-08-20 10:00:00'),
('Puzzle 500 animale',             5,  '2020-08-01', 59.99,   4, 6, 4,  10, '2023-09-05 11:00:00'),
('Joc Catan Junior',               6,  '2021-06-10', 119.99,  2, 5, 5,  2,  '2023-10-01 08:30:00'),
('Saltea gonflabila',              3,  '2020-04-25', 89.99,   6, 8, 6,  4,  '2023-10-15 09:30:00'),
('Robot programabil',              8,  '2021-09-05', 449.99,  3, 4, 7,  1,  '2023-11-01 10:30:00'),
('Set plastilina 24 culori',       3,  '2022-01-20', 39.99,   5, 7, 8,  8,  '2023-11-20 11:30:00'),
('Joc table lemn',                 7,  '2020-12-10', 199.99,  1, 1, 9,  2,  '2023-12-01 08:00:00'),
('Papusa bebelus interactiva',     0,  '2021-02-15', 249.99,  3, 4, 10, 6,  '2023-12-15 09:00:00'),
('Masinuta BMW electrica',         3,  '2020-06-15', 899.99,  2, 5, 1,  7,  '2023-01-10 08:00:00'),
('Set Lego Castle 900 piese',      8,  '2019-09-20', 319.99,  2, 5, 2,  5,  '2023-01-25 09:00:00'),
('Papusa Steffi cu accesorii',     3,  '2020-03-10', 119.99,  3, 4, 3,  3,  '2023-02-05 10:00:00'),
('Trotineta lemn bebe',            2,  '2021-01-20', 199.99,  7, 3, 4,  4,  '2023-02-20 11:00:00'),
('Joc sah si table set',           6,  '2020-07-05', 149.99,  1, 1, 5,  2,  '2023-03-01 08:00:00'),
('Puzzle glob pamantesc 540p',     8,  '2021-04-15', 99.99,   4, 6, 6,  10, '2023-03-15 09:00:00'),
('Figurina dinosaur T-Rex',        4,  '2020-10-25', 79.99,   3, 4, 7,  3,  '2023-04-01 10:00:00'),
('Telescop reflector copii',       10, '2021-08-10', 349.99,  2, 5, 8,  1,  '2023-04-15 11:00:00'),
('Xilofon lemn colorat',           1,  '2020-05-30', 69.99,   5, 7, 9,  6,  '2023-04-28 08:00:00'),
('Cuburi matematica magneti',      4,  '2021-11-05', 129.99,  3, 4, 10, 1,  '2023-05-10 09:00:00'),
('Set geografie harta puzzle',     7,  '2020-02-18', 89.99,   4, 6, 1,  10, '2023-05-25 10:00:00'),
('Trenulet lemn traseu mare',      3,  '2021-05-22', 249.99,  2, 5, 2,  7,  '2023-06-10 11:00:00'),
('Papusa articulata balerina',     5,  '2020-08-28', 109.99,  3, 4, 3,  3,  '2023-06-20 08:00:00'),
('Scuter copii cu lumini',         4,  '2021-07-12', 499.99,  3, 4, 4,  4,  '2023-07-05 09:00:00'),
('Joc memory animale',             3,  '2020-11-15', 44.99,   4, 6, 5,  2,  '2023-07-20 10:00:00'),
('Set creatie sapun artizanal',    7,  '2021-03-28', 74.99,   5, 7, 6,  8,  '2023-08-05 11:00:00'),
('Joc bowling copii lemn',         4,  '2020-09-10', 84.99,   1, 1, 7,  2,  '2023-08-18 08:00:00'),
('Figurine set animale salbatice', 3,  '2021-12-20', 59.99,   3, 4, 8,  3,  '2023-09-01 09:00:00'),
('Calculator solar copii',         8,  '2020-04-05', 39.99,   3, 4, 9,  9,  '2023-09-15 10:00:00'),
('Masinuta inertie 1:18',          3,  '2021-02-08', 49.99,   3, 4, 10, 7,  '2023-09-28 11:00:00'),
('Set joaca nisip kinetic 2kg',    3,  '2020-06-25', 99.99,   5, 7, 1,  8,  '2023-10-08 08:00:00'),
('Joc Battleship nave',            7,  '2021-09-18', 109.99,  2, 5, 2,  2,  '2023-10-22 09:00:00'),
('Papusa fashion cu hainute',      4,  '2020-12-05', 89.99,   3, 4, 3,  3,  '2023-11-05 10:00:00'),
('Placa snowboard copii',          5,  '2021-10-30', 299.99,  5, 7, 4,  4,  '2023-11-15 11:00:00'),
('Puzzle lemn animale 12p',        2,  '2020-03-22', 49.99,   1, 1, 5,  10, '2023-11-28 08:00:00'),
('Set pictura pe sticla',          6,  '2021-06-28', 64.99,   4, 6, 6,  8,  '2023-12-05 09:00:00'),
('Joc Pandemic boardgame',         8,  '2020-10-12', 139.99,  2, 5, 7,  2,  '2023-12-12 10:00:00'),
('Trenulet magnetic 30 piese',     2,  '2021-01-08', 179.99,  3, 4, 8,  7,  '2023-12-20 11:00:00'),
('Figurina dinozaur velociraptor', 4,  '2020-07-18', 69.99,   3, 4, 9,  3,  '2024-01-08 08:00:00'),
('Set experimente apa',            6,  '2021-04-02', 119.99,  2, 5, 10, 1,  '2024-01-15 09:00:00'),
('Papusa Skipper sora Barbie',     3,  '2020-09-28', 99.99,   3, 4, 1,  3,  '2024-01-22 10:00:00'),
('Joc Ludo 4 jucatori',            5,  '2021-08-22', 54.99,   1, 1, 2,  2,  '2024-01-30 11:00:00'),
('Masinuta metal vintage',         3,  '2020-05-15', 44.99,   7, 3, 3,  7,  '2024-02-05 08:00:00'),
('Set constructie poduri',         8,  '2021-11-28', 159.99,  2, 5, 4,  5,  '2024-02-12 09:00:00'),
('Pista curse masinute 3m',        5,  '2020-08-08', 229.99,  3, 4, 5,  7,  '2024-02-20 10:00:00'),
('Carte povesti interactive',      3,  '2021-07-15', 79.99,   1, 1, 6,  10, '2024-02-28 11:00:00'),
('Set instrumente muzicale bebe',  0,  '2020-11-02', 89.99,   3, 4, 7,  6,  '2024-03-06 08:00:00'),
('Joc darts magnetic copii',       6,  '2021-10-08', 74.99,   5, 7, 8,  2,  '2024-03-12 09:00:00'),
('Figurina robot transformabil',   7,  '2020-06-20', 119.99,  3, 4, 9,  3,  '2024-03-18 10:00:00'),
('Cort joaca interior copii',      2,  '2021-12-05', 189.99,  5, 7, 10, 4,  '2024-03-25 11:00:00');


-- VERIFYING DATA

SELECT * FROM Utilizator

SELECT * FROM Product

SELECT * FROM Purchase

SELECT * FROM Category

SELECT * FROM City

SELECT * FROM Country

SELECT * FROM Importator

SELECT * FROM OutOfStock

SELECT * FROM Payment

SELECT * FROM Shop