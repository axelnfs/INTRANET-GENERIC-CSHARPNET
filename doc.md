ProjectName/
│
├── Models/                          # DTOs y entidades
│   ├── DTOs/
│   │   ├── Request/
│   │   └── Response/
│   └── Entities/
│
├── Data/                            # Acceso a base de datos
│   ├── DatabaseHelper.cs            # Conexión estática
│   └── StoredProcedures/            # Clases para ejecutar SPs
│
├── Services/                        # Lógica de negocio (acceso estático)
│   └── [NombreServicio]Service.cs
│
├── Controllers/                     # Lógica de procesamiento
│   └── [Nombre]Controller.cs
│
├── Api/                             # Endpoints API REST
│   └── [Nombre]ApiController.cs
│
├── Webhooks/                        # Manejadores de webhooks
│   └── [Nombre]WebhookHandler.cs
│
├── wwwroot/                         # Contenido estático
│   ├── css/
│   ├── js/
│   │   ├── api/                     # Llamadas AJAX
│   │   └── pages/                   # Scripts por página
│   ├── images/
│   └── lib/
│
├── Views/                           # Vistas Razor
│   ├── Shared/
│   │   └── _Layout.cshtml
│   └── [Controller]/
│
└── Configuration/
    └── AppSettings.cs               # Configuración estática