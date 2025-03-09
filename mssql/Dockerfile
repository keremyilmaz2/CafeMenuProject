FROM mcr.microsoft.com/mssql/server:2022-latest

# Root yetkisi al
USER root

# Önce eski ve çakışan paketleri kaldır
RUN apt-get update && apt-get remove -y libodbc2 unixodbc-common && \
    apt-get autoremove -y && apt-get clean

# apt-get hatasını gidermek için eksik dizinleri oluştur ve izinleri değiştir
RUN mkdir -p /var/lib/apt/lists/partial && chmod -R 777 /var/lib/apt/lists

# SQLCMD ve MSSQL Tools yükleme
RUN apt-get update && apt-get install -y curl gnupg && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> /etc/bash.bashrc

# Restore scriptini konteynere kopyala ve çalıştırılabilir yap
COPY restore-db.sh /restore-db.sh
RUN chmod +x /restore-db.sh

# MSSQL kullanıcısına geri dön
USER mssql

# MSSQL başlatıldığında veritabanlarını restore et
CMD /opt/mssql/bin/sqlservr & sleep 20 && /restore-db.sh && wait
