# Тестовое задание ТомскАСУПроект

1. Для запуска тестов необходимо написать dotnet test из корня решения. Тесты используют In-Memory базу данных вместо MSSQL.
2. Для запуска запуска приложения необходимо:
2.1. Изменить DefaultConnection на ConnectionString вашей базы данных.
2.2. Изменить порт в appsettings.json, если это необходимо. Порт по умолчанию: 60000.
2.3. Выполнить dotnet run из папки проекта HallOfFame.API. После этого будет произведена миграция БД и добавление нескольких демо-сущностей.
2.4. Для доступа к интерфейсу Swagger необходимо перейти по http://localhost:60000/swagger

#### Описание эндпоинтов:
1. GET /api/v1/persons - возвращает список всех сущностей Person. 
2. GET /api/v1/person/{id} - возвращает сущность Person с идом id и связанные с ним Skill.
3. **POST** /api/v1/person/{id} - **изменяет** сущность Person с идом id.
4. **PUT** /api/v1/person - **добавляет** новую сущность Person.
5. DELETE /api/v1/person/{id} - удаляет сущность Person с идом id.