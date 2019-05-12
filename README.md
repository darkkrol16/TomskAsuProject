# Тестовое задание ТомскАСУПроект
### Развертывание
1. Для запуска тестов необходимо написать dotnet test из корня решения. Тесты используют In-Memory базу данных вместо MSSQL.
2. Для запуска запуска приложения необходимо:<br />
2.1. Изменить DefaultConnection на ConnectionString вашей базы данных в файле appsettings.json.<br />
2.2. Изменить порт в appsettings.json, если это необходимо. Порт по умолчанию: 60000.<br />
2.3. Выполнить dotnet run из папки проекта HallOfFame.API. После этого будет произведена миграция БД и добавление нескольких демо-сущностей.<br />
2.4. Для доступа к интерфейсу Swagger необходимо перейти по http://localhost:60000/swagger<br />

Требуется .NET Core 2.2

### Развертывание с помощью docker-compose
Если вы хотите развернуть этот проект с использованием docker-compose, вам необходимо:
1. Создать файл docker-compose.yml следующего содержания:

```
version: '3.4'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2017-latest-ubuntu
    environment:
      SA_PASSWORD: "Password123"
      ACCEPT_EULA: "Y"
    ports:
      - 1401:1433


  halloffame.api:
    image: darkkrol16/halloffame.api
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:80
    ports:
      - "60000:80"
    depends_on:
      - sqlserver
```

2. Вызвать docker-compose up из папки, в которой содержится файл docker-compose.yml
3. Перейти по http://localhost:60000

P.S. Так как в dockerhub репозиториях есть только MSSQL 2017 и MSSQL 2019, то было решено использовать MSSQL 2017 (согласно заданию требуется 2016).

### Описание эндпоинтов:
1. GET /api/v1/persons - возвращает список всех сущностей Person. 
2. GET /api/v1/person/{id} - возвращает сущность Person с идом id и связанные с ним Skill.
3. **POST** /api/v1/person/{id} - **изменяет** сущность Person с идом id.
4. **PUT** /api/v1/person - **добавляет** новую сущность Person.
5. DELETE /api/v1/person/{id} - удаляет сущность Person с идом id.
