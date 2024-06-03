dotnet ef migrations add "InitialMig" --startup-project mass-transit-bug.csproj  --output-dir Persistence/Migrations

1. They are 2 issues. one Consumer is never called unless ``config.UseBusOutbox();`` commented
2. Second after firing event by using endpoint ``/send-event``. Wait for a around 5 minutes, and it will throw that exception .
3. It should work tih ``x.DisableDeliveryService();``