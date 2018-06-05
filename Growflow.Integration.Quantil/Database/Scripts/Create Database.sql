﻿-- Script Date: 24/04/2018 17:46  - ErikEJ.SqlCeScripting version 3.5.2.75
CREATE TABLE [OnlineOrder] (
  [Id] INTEGER AUTOINCREMENT NOT NULL
, [GrowFloId] INTEGER NOT NULL
, [IsImported] INTEGER DEFAULT 0 NOT NULL
, [IsMarked] INTEGER DEFAULT 0 NOT NULL
, [Account_Ref] TEXT NOT NULL
, CONSTRAINT [PK_OnlineOrder] PRIMARY KEY ([Id])
);
