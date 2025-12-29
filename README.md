# iowa



Cần Cassandra không?: có

Blog Storage?: Không rõ

Cần SQL Server?: Có

Có service bus?: Có nhưng là message bus

Các data phải ở trạng thái migration như nào? (Như Cassandra phải có table này table kia khi kết nối): Với Cassandra thì cần có sự tồn tại của keyspace và Table



Tóm lại(+ A.I suggestion):

\- Cassandra cần có, và các thông tin như contact point, host, keyspace và table mapping (thứ như thế này: "\[Table("user\_id\_by\_subscription\_plan")]") phải đúng

\- Blob storage không cần và cũng không có

\- SQL server phải có 

\- Message bus có thiết lập nhưng không biết có thể coi nó là Service bus không 

