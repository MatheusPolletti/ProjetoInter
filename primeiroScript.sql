IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Instituicoes] (
    [InstituicaoId] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Endereco] nvarchar(max) NOT NULL,
    [Contato] nvarchar(max) NULL,
    CONSTRAINT [PK_Instituicoes] PRIMARY KEY ([InstituicaoId])
);
GO

CREATE TABLE [Setores] (
    [SetorId] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Setores] PRIMARY KEY ([SetorId])
);
GO

CREATE TABLE [Animais] (
    [AnimalId] int NOT NULL IDENTITY,
    [SetorId] int NOT NULL,
    [Nome] nvarchar(max) NOT NULL,
    [Especie] nvarchar(max) NOT NULL,
    [DataNascimento] date NULL,
    [ImagemUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_Animais] PRIMARY KEY ([AnimalId]),
    CONSTRAINT [FK_Animais_Setores_SetorId] FOREIGN KEY ([SetorId]) REFERENCES [Setores] ([SetorId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Funcionarios] (
    [FuncionarioId] int NOT NULL IDENTITY,
    [SetorId] int NOT NULL,
    [Nome] nvarchar(max) NOT NULL,
    [Cpf] nvarchar(max) NOT NULL,
    [Cargo] nvarchar(max) NOT NULL,
    [Telefone] nvarchar(max) NULL,
    CONSTRAINT [PK_Funcionarios] PRIMARY KEY ([FuncionarioId]),
    CONSTRAINT [FK_Funcionarios_Setores_SetorId] FOREIGN KEY ([SetorId]) REFERENCES [Setores] ([SetorId])
);
GO

CREATE TABLE [HistoricosSaude] (
    [HistoricoSaudeId] int NOT NULL IDENTITY,
    [AnimalId] int NOT NULL,
    [TipoEvento] nvarchar(max) NOT NULL,
    [Data] datetime2 NOT NULL,
    [EstadoSaude] nvarchar(max) NOT NULL,
    [Observacoes] nvarchar(max) NULL,
    CONSTRAINT [PK_HistoricosSaude] PRIMARY KEY ([HistoricoSaudeId]),
    CONSTRAINT [FK_HistoricosSaude_Animais_AnimalId] FOREIGN KEY ([AnimalId]) REFERENCES [Animais] ([AnimalId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Transferencias] (
    [TransferenciaId] int NOT NULL IDENTITY,
    [AnimalId] int NOT NULL,
    [InstituicaoOrigemInstituicaoId] int NOT NULL,
    [InstituicaoDestinoInstituicaoId] int NOT NULL,
    [DataEntrada] datetime2 NOT NULL,
    [DataSaida] datetime2 NOT NULL,
    CONSTRAINT [PK_Transferencias] PRIMARY KEY ([TransferenciaId]),
    CONSTRAINT [FK_Transferencias_Animais_AnimalId] FOREIGN KEY ([AnimalId]) REFERENCES [Animais] ([AnimalId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Transferencias_Instituicoes_InstituicaoDestinoInstituicaoId] FOREIGN KEY ([InstituicaoDestinoInstituicaoId]) REFERENCES [Instituicoes] ([InstituicaoId]),
    CONSTRAINT [FK_Transferencias_Instituicoes_InstituicaoOrigemInstituicaoId] FOREIGN KEY ([InstituicaoOrigemInstituicaoId]) REFERENCES [Instituicoes] ([InstituicaoId])
);
GO

CREATE TABLE [AtendimentosVeterinarios] (
    [AtendimentoVeterinarioId] int NOT NULL IDENTITY,
    [AnimalId] int NOT NULL,
    [FuncionarioId] int NOT NULL,
    [Data] datetime2 NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    [Resultado] nvarchar(max) NOT NULL,
    [Observacoes] nvarchar(max) NULL,
    CONSTRAINT [PK_AtendimentosVeterinarios] PRIMARY KEY ([AtendimentoVeterinarioId]),
    CONSTRAINT [FK_AtendimentosVeterinarios_Animais_AnimalId] FOREIGN KEY ([AnimalId]) REFERENCES [Animais] ([AnimalId]),
    CONSTRAINT [FK_AtendimentosVeterinarios_Funcionarios_FuncionarioId] FOREIGN KEY ([FuncionarioId]) REFERENCES [Funcionarios] ([FuncionarioId])
);
GO

CREATE TABLE [Procedimentos] (
    [ProcedimentoId] int NOT NULL IDENTITY,
    [AnimalId] int NOT NULL,
    [FuncionarioId] int NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    [DataProcedimento] datetime2 NOT NULL,
    CONSTRAINT [PK_Procedimentos] PRIMARY KEY ([ProcedimentoId]),
    CONSTRAINT [FK_Procedimentos_Animais_AnimalId] FOREIGN KEY ([AnimalId]) REFERENCES [Animais] ([AnimalId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Procedimentos_Funcionarios_FuncionarioId] FOREIGN KEY ([FuncionarioId]) REFERENCES [Funcionarios] ([FuncionarioId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Animais_SetorId] ON [Animais] ([SetorId]);
GO

CREATE INDEX [IX_AtendimentosVeterinarios_AnimalId] ON [AtendimentosVeterinarios] ([AnimalId]);
GO

CREATE INDEX [IX_AtendimentosVeterinarios_FuncionarioId] ON [AtendimentosVeterinarios] ([FuncionarioId]);
GO

CREATE INDEX [IX_Funcionarios_SetorId] ON [Funcionarios] ([SetorId]);
GO

CREATE INDEX [IX_HistoricosSaude_AnimalId] ON [HistoricosSaude] ([AnimalId]);
GO

CREATE INDEX [IX_Procedimentos_AnimalId] ON [Procedimentos] ([AnimalId]);
GO

CREATE INDEX [IX_Procedimentos_FuncionarioId] ON [Procedimentos] ([FuncionarioId]);
GO

CREATE INDEX [IX_Transferencias_AnimalId] ON [Transferencias] ([AnimalId]);
GO

CREATE INDEX [IX_Transferencias_InstituicaoDestinoInstituicaoId] ON [Transferencias] ([InstituicaoDestinoInstituicaoId]);
GO

CREATE INDEX [IX_Transferencias_InstituicaoOrigemInstituicaoId] ON [Transferencias] ([InstituicaoOrigemInstituicaoId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250423194224_PrimeiraMigracao', N'8.0.15');
GO

COMMIT;
GO

