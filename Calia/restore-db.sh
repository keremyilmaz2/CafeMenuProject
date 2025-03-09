#!/bin/bash

# MSSQL'in başlatılmasını bekle
echo "MSSQL'in başlaması bekleniyor..."
sleep 20s

# SQL komutlarını çalıştır
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Keremkerem11!' -Q "
RESTORE DATABASE AuthDB FROM DISK = '/var/opt/mssql/backups/Calia_AuthAPI.bak' WITH MOVE 'AuthDB' TO '/var/opt/mssql/data/AuthDB.mdf', MOVE 'AuthDB_Log' TO '/var/opt/mssql/data/AuthDB.ldf', REPLACE;
RESTORE DATABASE CategoryDB FROM DISK = '/var/opt/mssql/backups/Calia_CategoryAPI.bak' WITH MOVE 'CategoryDB' TO '/var/opt/mssql/data/CategoryDB.mdf', MOVE 'CategoryDB_Log' TO '/var/opt/mssql/data/CategoryDB.ldf', REPLACE;
RESTORE DATABASE OrderDB FROM DISK = '/var/opt/mssql/backups/Calia_OrderAPI.bak' WITH MOVE 'OrderDB' TO '/var/opt/mssql/data/OrderDB.mdf', MOVE 'OrderDB_Log' TO '/var/opt/mssql/data/OrderDB.ldf', REPLACE;
RESTORE DATABASE ProductDB FROM DISK = '/var/opt/mssql/backups/Calia_ProductAPI.bak' WITH MOVE 'ProductDB' TO '/var/opt/mssql/data/ProductDB.mdf', MOVE 'ProductDB_Log' TO '/var/opt/mssql/data/ProductDB.ldf', REPLACE;
RESTORE DATABASE ShoppingCartDB FROM DISK = '/var/opt/mssql/backups/Calia_ShoppingCartAPI.bak' WITH MOVE 'ShoppingCartDB' TO '/var/opt/mssql/data/ShoppingCartDB.mdf', MOVE 'ShoppingCartDB_Log' TO '/var/opt/mssql/data/ShoppingCartDB.ldf', REPLACE;
RESTORE DATABASE StockDB FROM DISK = '/var/opt/mssql/backups/Calia_StockAPI.bak' WITH MOVE 'StockDB' TO '/var/opt/mssql/data/StockDB.mdf', MOVE 'StockDB_Log' TO '/var/opt/mssql/data/StockDB.ldf', REPLACE;
"

echo "Tüm veritabanları başarıyla restore edildi!"
