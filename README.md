# AccelokaWebApi Doccumentation
## **Database Convention:**

The database uses PostgreSQL. The naming convention for tables and columns is snake_case. Table names use plural words, while column names use singular words.
Based on the requirements, the users/customers table will not be created because there are no endpoints or specifications that require this table.
Also, since it is not explicitly stated, the date/time format in the database uses timestampz with the UTC timezone. The API then formats it as requested in the problem requirement, using the format dd-MM-yyyy HH:mm, but still in UTC. However, ideally the API response should return the date with an offset so the Front End can convert it to the desired timezone.
The .Sql backup file is in .\Acceloka.WebApiStandard.Entities\Sql\acceloka_export_sql.sql

**Tables**
1. Tickets

   For Tickets, the PK is using ticket_code based on first letter of every word in the ticket category name and 3 digit sequence number starting from 001. For example, the ticket_code for first ticket with category: "Transportasi Laut" will be TL001, the second one will be TL002 and so on. This will be a problem for automatically added row in database because the PK is not id with sequence 1,2,3...

   There are a solutions for this problem (future work),
   1. We can add both the first letter of every word in the ticket category name and ticket sequence to categories table, so a category will hold it's prefix and the last sequence of ticket in that category. For every ticket insertion transaction, we can increase the number of sequence by 1 for the ticket category. This can be done in AddTicketHandler if there's requirement in the future.
   2. On each ticket insertion, increment `last_seq` atomically (e.g., `UPDATE categories SET last_seq = last_seq + 1 RETURNING last_seq`) and then generate `ticket_code = prefix + seq.PadLeft(3, '0')`. This avoids race conditions under concurrent inserts

   However, since the current exam requirements do not include any endpoint for inserting new tickets, this feature is considered out of scope and is not implemented in the current database.
```
ticket_code varchar(10) primary key,
name varchar(255) not null,
event_date timestamptz not null,
price numeric(10,2) not null,
quota integer not null check (quota >= 0),
category_id integer references categories(id),
created_at timestamptz not null default now(),
created_by varchar(255) not null default 'System',
updated_at timestamptz not null default now(),
updated_by varchar(255) not null default 'System'
```
2. categories
```
id serial primary key,
name varchar(255) not null,
created_at timestamptz not null default now(),
created_by varchar(255) not null default 'System',
updated_at timestamptz not null default now(),
updated_by varchar(255) not null default 'System'
```
3. bookings
```
id serial primary key,
booking_date timestamptz not null default now(),
created_at timestamptz not null default now(),
created_by varchar(255) not null default 'System',
updated_at timestamptz not null default now(),
updated_by varchar(255) not null default 'System'
```
4. booking_tickets
booking_tickets has an ON DELETE CASCADE relationship with bookings because booking_tickets is a pivot table that fully depends on bookings; if a booking row is deleted, logically all related booking_tickets rows should also be deleted.
```
booking_id integer references bookings(id) on delete cascade,
ticket_code varchar(10) references tickets(ticket_code),
quantity integer not null default 1 check (quantity > 0),
created_at timestamptz not null default now(),
created_by varchar(255) not null default 'System',
updated_at timestamptz not null default now(),
updated_by varchar(255) not null default 'System',
primary key(booking_id, ticket_code)
```

ERD:

<img width="1434" height="614" alt="Untitled(3)" src="https://github.com/user-attachments/assets/3bbbd0c5-8c1b-4f40-99e5-76e8249899bb" />

## **Clean Code Architecture and MARVEL Pattern implementation**

This API architecture is based on https://github.com/accelist/Accelist.WebApiStandard.
In the solution, there are 4 projects that correspond to the layers in Clean Code Architecture:
* **Entities Layer**: Contains entities that are mapped directly to database objects via scaffolding
* **Contracts Layer**: Contains a collection of classes used as MediatR requests and responses
* **Core Layer**: Contains the main business logic used by the API, including handlers, validators, and a FluentValidation + MediatR pipeline to integrate both
* **App Layer**: Maps the Core layer to the system’s API interface, which is a JSON Web API

## **Case Requirement**

Logging has been set up using the Serilog sink file, configured in Program.cs. In each handler, information is logged via the DI-injected logger using the _logger.LogInformation() method.
Log is stored in "Log" folder which will be automatically created when running the API:

<img width="340" height="105" alt="Screenshot 2026-02-12 221645" src="https://github.com/user-attachments/assets/f1046af0-6303-426f-b0c2-71a185a45f8e" />

The API has also implemented MediatR and FluentValidation, both have been integrated via MediatR Pipeline middleware in Acceloka.WebApiStandard/Commons/Behaviors/ValidationBehavior.cs which will run the validator before every handler is executed.
The Input in API route,query, or body is case sensitive for PK, for the consideration that every key is unique, so this is enforced in the validator. For example, ticket code “TO001” and “To001” will be interpreted as different.

RFC 7807 standard has also been implemented through ErrorController which is registered in Program.cs. Every error will be redirected to ./error path in ErrorController which is configured to catch ValidationException thrown by FluentValidation in ValidationBehavior.cs and any other error formatted to RFC 7807 standard via return Problem(instance: originalPath);

Every EF Core Query via database DI has used async await.

## **API Endpoint response example**

1. **View Available Ticket**

   Pagination is applied here where you can enter the page size and which page to want to access

   Error Route Example:

   <img width="928" height="459" alt="Screenshot 2026-02-12 215508" src="https://github.com/user-attachments/assets/39dc6ebc-c5dc-472a-9ab0-e07878a5a65a" />

   Correct Route Example:

   <img width="1087" height="852" alt="Screenshot 2026-02-12 215814" src="https://github.com/user-attachments/assets/d9925951-1e85-42bc-8faf-1c606c9e6326" />

2. **Book Ticket**

   Error Route Example:

   <img width="763" height="683" alt="Screenshot 2026-02-12 210530" src="https://github.com/user-attachments/assets/2564548f-25b1-4767-add3-26c9391b1bc3" />
   
   <img width="663" height="637" alt="Screenshot 2026-02-12 210629" src="https://github.com/user-attachments/assets/416d51e9-831e-49d5-8a67-9ab31d3c285f" />
   
   Correct Route Example:

   <img width="784" height="864" alt="Screenshot 2026-02-12 213658" src="https://github.com/user-attachments/assets/42d7186d-142c-4975-b2fc-c745b287ac04" />

   <img width="1496" height="336" alt="Screenshot 2026-02-12 213801" src="https://github.com/user-attachments/assets/3f5f972d-617d-4915-95f5-b452537201fa" />

3. **View Ticket Detail**

   Error Route Example:

   <img width="608" height="531" alt="Screenshot 2026-02-12 214025" src="https://github.com/user-attachments/assets/9cde9e25-2846-4c59-bef5-f67782d2c517" />

   Correct Route Example:

   <img width="660" height="824" alt="Screenshot 2026-02-12 214119" src="https://github.com/user-attachments/assets/168d0f5a-854e-486c-b5f6-2e797b5ed144" />

4. **Revoke Ticket**

   In this endpoint, the requirement response contract does differ from the reference picture given, hence the response of this endpoint will follow the written response requirements instead where the response is a single object made of TicketCode, CodeName, CategoryName, and Quantity left. Apart from that, there’s also adjustment to the removing algorithm. Since the database consist of table bookings (this table holds the id PK and booking_date) and booking_tickets (this table act as the pivot between booking and tickets, and represents tickets that’s been booked), if there are only a single tickets in certain booking that’s been revoked to 0 quantity, the booking_tickets row will be removed along with bookings row since there are no more tickets that’s been booked in that booking Id.
   
   Error Route Example:

   <img width="539" height="396" alt="Screenshot 2026-02-12 214946" src="https://github.com/user-attachments/assets/8b911403-85a8-49f6-909a-1cf851478682" />

   <img width="627" height="411" alt="Screenshot 2026-02-12 215019" src="https://github.com/user-attachments/assets/c3049705-1dc4-4663-86db-132069e39ca4" />

   Correct Route Example:

   Here, the quantity left is 0, the booking_ticket row for TicketCode “TL002” will be deleted, but since the booking still holds other tickets. The entirety of the booking id is not deleted.

   <img width="600" height="338" alt="Screenshot 2026-02-12 215148" src="https://github.com/user-attachments/assets/e09a4be0-e612-443a-90ee-f45e81f5f04e" />

   <img width="1434" height="303" alt="Screenshot 2026-02-12 215330" src="https://github.com/user-attachments/assets/99a9a11b-634d-4c5c-8a63-af3f50d1e3f8" />


5. **Edit Ticket Quantity**
    
    There is a bit of ambiguity in the requirements. When we edit the booked ticket quantity, although not mentioned, the assumption made is that the ticket quota is also changed according to the new quantity of the booked ticket
The ticket quota is now - the difference from the new qty - old qty
For example, if there are 7 remaining quota of the ticket, 3 has been booked and then changed to 5. The quota is reduced by 2 with 5 remaining
If there are 7 remaining quota, 5 have been booked and changed to 3. The quota is 3-5 = (-2) 7-(-2) so there are 9 remaining
    
   Error Route Example:

   <img width="605" height="550" alt="Screenshot 2026-02-12 220045" src="https://github.com/user-attachments/assets/9220bf30-288a-4462-8e7f-311d851b5790" />

   <img width="669" height="557" alt="Screenshot 2026-02-12 220119" src="https://github.com/user-attachments/assets/a7600785-6c38-4d5a-b234-aa7c8bcb9d05" />

   Correct Route Example:

   <img width="674" height="593" alt="Screenshot 2026-02-12 220153" src="https://github.com/user-attachments/assets/f1e52101-a341-43f7-824d-f292d3847c84" />

   
