# csharp.examples

The examples all use MSSQL Server so that 
```powershell
dotnet tool install --global dotnet-ef
// make sure db exists -- i.e. Blog
dotnet ef database update
```

To create the migration and generate the sql
```powershell
cd src/EntityFrameworkCore
dotnet ef migrations add "Initial"
dotnet ef database update --project .\Relationships\Relationships.csproj
dotnet ef migrations script --no-build --idempotent --project .\Relationships\Relationships.csproj --output sql/table/EFCore.migration.sql
```

## Relationships

The two relationships in this model are:

* Each Order can have many Items (one-to-many):
    * Order is an aggregate root.
    * OrderItem a dependent/child. It contains the FK property OrderItem.OrderId, the value of which must match the Order.OrderId PK value of the related order.
    * Order.Items is a collection navigation from an order to all the associated items.  There is no inverse mapping to get from child to parent.
* Each Order can have one Address (one-to-one):
    * Order is the aggregate root.
    * Address is the dependent/child. Order has a reference navigation to the Address.  There is no inverse navigation from Address to Order
    * Order implies an FK to Address in the table definition, but does not expose AddressId outside of the Address child.   
* Each post can have many tags and each tag can have many posts (many-to-many):
    * Many-to-many relationships are a further layer over two one-to-many relationships. Many-to-many relationships are covered later in this document.
    * Post.Tags is a collection navigation from a post to all the associated tags. Post.Tags is the inverse navigation for Tag.Posts.
    * Tag.Posts is a collection navigation from a tag to all the associated posts. Tag.Posts is the inverse navigation for Post.Tags.

