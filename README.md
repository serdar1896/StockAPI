# StockAPI
Projenin Amacı:
 Stok entegrasyonunu sağlayarak asenkron bir şekilde database ürünleri aktaran ve yine 
aynı source’lardan sorgulama yapabilen bir API projesidir.

Kullanılan Teknolojiler: .Net Core 3.1 , MongoDB , Redis Cache,Swagger, RabbitMQ, AutoMapper , AutoFac ,Json

Proje Detayı: 
Proje incelenirken kolaylık olması adına Redis , rabbitMQ ve mongoDB Cloud’a 
kurulmuştur. 
Projede, her stok girişinde ayni productCode ve variantCode’a sahip bir ürün var ise 
girilen stok var olan stok sayısının üzerine eklenmektedir.
Eğer var olan bir ürün bilgisi yok ise girilen bilgilere göre yeni bir ürün oluşturulmaktadır. 
