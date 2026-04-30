CREATE TABLE IF NOT EXISTS transactions (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    type             TEXT    NOT NULL,
    transaction_id   TEXT,
    amount           REAL,
    currency         TEXT,
    raw_message      TEXT    NOT NULL,
    received_at      TEXT    NOT NULL DEFAULT (datetime('now'))
);