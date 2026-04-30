Assumptions:
- SQLite je koristen jer ne zahtjeva kompleksniji setup (schema je u jednom file-u), međutim na produkcijskoj okolini s više klijenata PostgreSQL bi bio bolji izbor zbog konkurentnosti
- Radi 1 instanca servera
- Output se pise prvo u file, pa u bazu

Tradeoffs:
- FrameParser.ReadNext() radi sinkrono - async bi bilo bolje
- Konkurentni pristup file-u output.txt nije implementiran - rjesenje bi bio shared writer s lock-om
- CREATE TABLE se poziva pri svakoj novoj incijalizaciji TransactionRepository (ovo nije problem zbog IF NOT EXISTS)
- TransactionRepositoryu se pristupa direktno, a ne preko dependency injectiona - ovo je pojednostavljenje za ovaj case
