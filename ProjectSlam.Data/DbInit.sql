CREATE TABLE IF NOT EXISTS GlobalMoveset (
    MoveId INTEGER PRIMARY KEY AUTOINCREMENT,
    MoveCategory TEXT NOT NULL,
    MoveName  TEXT NOT NULL UNIQUE COLLATE NOCASE,
    DamageAmount INTEGER NOT NULL,
    IsFinisher INTEGER NOT NULL,
    IsSignature INTEGER NOT NULL);

CREATE INDEX IF NOT EXISTS IX_GlobalMoveset_MoveName ON GlobalMoveset (MoveName);
CREATE INDEX IF NOT EXISTS IX_GlobalMoveset_MoveCategory ON GlobalMoveset (MoveCategory);