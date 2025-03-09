#!/bin/bash

echo "MSSQL'in başlaması bekleniyor..."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Keremkerem11!' -Q "SELECT name FROM sys.databases;" &> /dev/null
do
  sleep 5
  echo "Bekleniyor..."
done

echo "MSSQL Başladı! Geri yükleme başlıyor..."

for db in Auth Category Order Product ShoppingCart Stock
do
  BAK_FILE="/var/opt/mssql/backups/Calia_${db}API.bak"

  # Eğer .bak dosyası yoksa bu veritabanını atla
  if [ ! -f "$BAK_FILE" ]; then
    echo "UYARI: $BAK_FILE bulunamadı, $dbDB geri yüklenmeyecek!"
    continue
  fi

  echo "$dbDB için dosya bilgileri alınıyor..."

  # Veritabanı dosya isimlerini almak için `RESTORE FILELISTONLY` komutu çalıştır
  FILE_INFO=$(/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Keremkerem11!' -Q "RESTORE FILELISTONLY FROM DISK = '$BAK_FILE';" -s "|" -W | tail -n +3)

  MDF_LOGICAL_NAME=$(echo "$FILE_INFO" | awk -F "|" '{print $1}' | xargs | head -n 1)
  LDF_LOGICAL_NAME=$(echo "$FILE_INFO" | awk -F "|" '{print $1}' | xargs | tail -n 1)

  echo "MDF Logical Name: $MDF_LOGICAL_NAME"
  echo "LDF Logical Name: $LDF_LOGICAL_NAME"

  echo "$dbDB geri yükleniyor..."
  
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Keremkerem11!' -Q "
  RESTORE DATABASE ${db}DB 
  FROM DISK = '$BAK_FILE'
  WITH MOVE '$MDF_LOGICAL_NAME' TO '/var/opt/mssql/data/${db}DB.mdf',
       MOVE '$LDF_LOGICAL_NAME' TO '/var/opt/mssql/data/${db}DB.ldf',
       REPLACE;"
  
  # Eğer hata olursa durdur
  if [ $? -ne 0 ]; then
    echo "HATA: $dbDB geri yüklenirken bir sorun oluştu!"
    exit 1
  fi

  echo "$dbDB başarıyla geri yüklendi!"
done

echo "Tüm veritabanları başarıyla geri yüklendi!"
