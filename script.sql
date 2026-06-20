CREATE DATABASE IF NOT EXISTS AgenciaViagens;

USE AgenciaViagens;

CREATE TABLE IF NOT EXISTS Usuario (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    SenhaHash VARCHAR(255) NOT NULL,
    NivelAcesso VARCHAR(50) NOT NULL,
    Telefone VARCHAR(20),
    Cpf VARCHAR(14),
    Cep VARCHAR(8),
    DataNascimento DATE
);

CREATE TABLE IF NOT EXISTS Voo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Origem VARCHAR(100) NOT NULL,
    OrigemUf VARCHAR(2) NOT NULL,
    Destino VARCHAR(100) NOT NULL,
    DestinoUf VARCHAR(2) NOT NULL,
    DataViagem DATE NOT NULL,
    Preco DECIMAL(10,2) NOT NULL,
    NumPassagens INT NOT NULL
);

CREATE TABLE IF NOT EXISTS Compra (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    VooId INT NOT NULL,
    DataCompra DATETIME DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT FK_Compra_Usuario
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(Id),

    CONSTRAINT FK_Compra_Passagem
        FOREIGN KEY (VooId)
        REFERENCES Voo(Id)
);

INSERT INTO Voo (Origem, OrigemUf, Destino, DestinoUf, DataViagem, Preco, NumPassagens) VALUES 
('São Paulo', 'SP', 'Rio de Janeiro', 'RJ', DATE_ADD(CURDATE(), INTERVAL 10 DAY), 350.00, 45),
('Belo Horizonte', 'MG', 'Salvador', 'BA', DATE_ADD(CURDATE(), INTERVAL 15 DAY), 480.00, 30),
('Brasília', 'DF', 'Florianópolis', 'SC', DATE_ADD(CURDATE(), INTERVAL 8 DAY), 620.00, 25),
('Recife', 'PE', 'Fernando de Noronha', 'PE', DATE_ADD(CURDATE(), INTERVAL 5 DAY), 850.00, 15),
('Porto Alegre', 'RS', 'São Paulo', 'SP', DATE_ADD(CURDATE(), INTERVAL 12 DAY), 290.00, 50),
('Curitiba', 'PR', 'Rio de Janeiro', 'RJ', DATE_ADD(CURDATE(), INTERVAL 7 DAY), 320.00, 40),
('Fortaleza', 'CE', 'Natal', 'RN', DATE_ADD(CURDATE(), INTERVAL 20 DAY), 190.00, 60),
('Manaus', 'AM', 'Belém', 'PA', DATE_ADD(CURDATE(), INTERVAL 14 DAY), 410.00, 35),
('Salvador', 'BA', 'Rio de Janeiro', 'RJ', DATE_ADD(CURDATE(), INTERVAL 18 DAY), 450.00, 28),
('Brasília', 'DF', 'Rio de Janeiro', 'RJ', DATE_ADD(CURDATE(), INTERVAL 9 DAY), 380.00, 32),
('São Paulo', 'SP', 'Salvador', 'BA', DATE_ADD(CURDATE(), INTERVAL 6 DAY), 540.00, 38),
('Rio de Janeiro', 'RJ', 'Recife', 'PE', DATE_ADD(CURDATE(), INTERVAL 22 DAY), 470.00, 42),
('São Paulo', 'SP', 'Belo Horizonte', 'MG', DATE_ADD(CURDATE(), INTERVAL 4 DAY), 210.00, 55),
('Rio de Janeiro', 'RJ', 'Florianópolis', 'SC', DATE_ADD(CURDATE(), INTERVAL 16 DAY), 390.00, 33),
('São Paulo', 'SP', 'Fortaleza', 'CE', DATE_ADD(CURDATE(), INTERVAL 25 DAY), 680.00, 20),
('São Paulo', 'SP', 'Manaus', 'AM', DATE_ADD(CURDATE(), INTERVAL 30 DAY), 920.00, 18),
('Goiânia', 'GO', 'Brasília', 'DF', DATE_ADD(CURDATE(), INTERVAL 3 DAY), 175.00, 48),
('Vitória', 'ES', 'São Paulo', 'SP', DATE_ADD(CURDATE(), INTERVAL 11 DAY), 260.00, 44),
('Maceió', 'AL', 'Recife', 'PE', DATE_ADD(CURDATE(), INTERVAL 13 DAY), 220.00, 50),
('João Pessoa', 'PB', 'Fortaleza', 'CE', DATE_ADD(CURDATE(), INTERVAL 17 DAY), 240.00, 36),
('Teresina', 'PI', 'São Luís', 'MA', DATE_ADD(CURDATE(), INTERVAL 19 DAY), 310.00, 27),
('Cuiabá', 'MT', 'Campo Grande', 'MS', DATE_ADD(CURDATE(), INTERVAL 21 DAY), 280.00, 39),
('Aracaju', 'SE', 'Salvador', 'BA', DATE_ADD(CURDATE(), INTERVAL 6 DAY), 195.00, 53),
('Palmas', 'TO', 'Brasília', 'DF', DATE_ADD(CURDATE(), INTERVAL 23 DAY), 330.00, 31),
('Macapá', 'AP', 'Belém', 'PA', DATE_ADD(CURDATE(), INTERVAL 28 DAY), 360.00, 24),
('Boa Vista', 'RR', 'Manaus', 'AM', DATE_ADD(CURDATE(), INTERVAL 26 DAY), 400.00, 22),
('Rio Branco', 'AC', 'Porto Velho', 'RO', DATE_ADD(CURDATE(), INTERVAL 24 DAY), 370.00, 26),
('Porto Velho', 'RO', 'Cuiabá', 'MT', DATE_ADD(CURDATE(), INTERVAL 9 DAY), 420.00, 29),
('Belém', 'PA', 'São Luís', 'MA', DATE_ADD(CURDATE(), INTERVAL 14 DAY), 300.00, 34),
('Rio de Janeiro', 'RJ', 'Belo Horizonte', 'MG', DATE_ADD(CURDATE(), INTERVAL 5 DAY), 230.00, 52),
('São Paulo', 'SP', 'Porto Alegre', 'RS', DATE_ADD(CURDATE(), INTERVAL 12 DAY), 310.00, 47),
('São Paulo', 'SP', 'Curitiba', 'PR', DATE_ADD(CURDATE(), INTERVAL 7 DAY), 200.00, 58),
('Rio de Janeiro', 'RJ', 'Fortaleza', 'CE', DATE_ADD(CURDATE(), INTERVAL 29 DAY), 640.00, 21),
('Salvador', 'BA', 'Fortaleza', 'CE', DATE_ADD(CURDATE(), INTERVAL 15 DAY), 380.00, 37),
('Recife', 'PE', 'Salvador', 'BA', DATE_ADD(CURDATE(), INTERVAL 10 DAY), 290.00, 41),
('Brasília', 'DF', 'Salvador', 'BA', DATE_ADD(CURDATE(), INTERVAL 8 DAY), 440.00, 30),
('Belo Horizonte', 'MG', 'Brasília', 'DF', DATE_ADD(CURDATE(), INTERVAL 6 DAY), 280.00, 49),
('Florianópolis', 'SC', 'Porto Alegre', 'RS', DATE_ADD(CURDATE(), INTERVAL 4 DAY), 160.00, 60),
('Natal', 'RN', 'Recife', 'PE', DATE_ADD(CURDATE(), INTERVAL 9 DAY), 170.00, 56),
('São Paulo', 'SP', 'Recife', 'PE', DATE_ADD(CURDATE(), INTERVAL 18 DAY), 590.00, 23),
('Rio de Janeiro', 'RJ', 'Manaus', 'AM', DATE_ADD(CURDATE(), INTERVAL 35 DAY), 980.00, 16),
('Curitiba', 'PR', 'Florianópolis', 'SC', DATE_ADD(CURDATE(), INTERVAL 5 DAY), 150.00, 62),
('São Paulo', 'SP', 'Vitória', 'ES', DATE_ADD(CURDATE(), INTERVAL 9 DAY), 270.00, 46),
('Belém', 'PA', 'Manaus', 'AM', DATE_ADD(CURDATE(), INTERVAL 13 DAY), 340.00, 33),
('Salvador', 'BA', 'Brasília', 'DF', DATE_ADD(CURDATE(), INTERVAL 11 DAY), 460.00, 28),
('Fortaleza', 'CE', 'São Luís', 'MA', DATE_ADD(CURDATE(), INTERVAL 16 DAY), 320.00, 35),
('Goiânia', 'GO', 'São Paulo', 'SP', DATE_ADD(CURDATE(), INTERVAL 7 DAY), 240.00, 51),
('Campo Grande', 'MS', 'São Paulo', 'SP', DATE_ADD(CURDATE(), INTERVAL 14 DAY), 310.00, 40),
('São Paulo', 'SP', 'Fernando de Noronha', 'PE', DATE_ADD(CURDATE(), INTERVAL 40 DAY), 1100.00, 12);

SELECT * FROM Usuario;
SELECT * FROM Voo;
SELECT * FROM Compra;