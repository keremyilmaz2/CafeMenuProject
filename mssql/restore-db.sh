#!/bin/bash

echo "MSSQL'in başlaması bekleniyor..."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Keremkerem11!' -Q "SELECT 1" &> /dev/null
do
  sleep 5
  echo "Bekleniyor..."
done

echo "MSSQL Başladı! Geri yükleme başlıyor..."

for db in Auth Category Order Product ShoppingCart Stock
do
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Keremkerem11!' -Q "
  RESTORE DATABASE ${db}DB FROM DISK = '/var/opt/mssql/backups/Calia_${db}API.bak' 
  WITH MOVE 'Calia_${db}API' TO '/var/opt/mssql/data/${db}DB.mdf', 
       MOVE 'Calia_${db}API_log' TO '/var/opt/mssql/data/${db}DB.ldf', REPLACE;"
done

echo "Tüm veritabanları başarıyla geri yüklendi!"
