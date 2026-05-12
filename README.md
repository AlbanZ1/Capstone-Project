# AuctionsApp - ASP.NET Core MVC Project

## 📌 Project Overview

AuctionsApp is a web-based auction platform built using **ASP.NET Core MVC** and **Entity Framework Core**. The application enables users to create, manage, and participate in online auctions. The platform offers functionalities such as image uploads for listings, a dynamic bidding system, and secure user authentication. This project serves as a demonstration of modern web development practices and cloud service integration. The backend database is hosted on **Azure SQL Database**, and images are stored in **Azure Blob Storage**, ensuring scalability and remote accessibility.

## 🚀 Features

- User authentication (Register/Login)
- Create and manage auction listings
- Upload and display listing images using **Azure Blob Storage**
- Bidding System/Functionality
- Secure SQL database integration
- Responsive Design

## 💻 Technologies Used

- **Backend**: C#, .NET Core, ASP.NET MVC
- **Frontend**: Razor Views, HTML, CSS, JavaScript
- **Database**: SQL Server
- **Cloud Storage**: Azure Blob Storage
- **Authentication**: ASP.NET Identity
- **Dependency Injection**: Built-in .NET Core DI
- **Version Control**: Git, GitHub

---

## ⚙️ Setup Instructions

### **1. Prerequisites**

Before running the project, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup) (Optional for database management)
- [Azure Account](https://portal.azure.com) (For database hosting and blob storage hosting)
- Git (for cloning the repository)

### **2. Clone the Repository**

```sh
 git clone https://github.com/your-username/AuctionsApp.git
 cd AuctionsApp
```

### **3. Install Dependencies**

```sh
 dotnet restore
```

---

## 👨‍💻 Usage Instructions

- ### Create an Account:
   - Register a new account or log in with existing credentials.

- ### Create a Listing:
   - Navigate to "Create Listing" and complete the form with the necessary details.

- ### Upload Images:
   - Use the provided interface to upload images for your listing.

- ### Bid on Items:
   - Browse available listings and place bids on items of interest.

- ### Manage Listings:
   - Edit or delete your listings from the "My Listings" section.

---
## 🛠 Database Configuration

### **Option 1: Use Azure SQL Database (Recommended)**

1. **Create an Azure SQL Database**

   - Sign in to [Azure Portal](https://portal.azure.com).
   - Create a new **SQL Database** with a new **SQL Server**.
   - Set **Firewall Rules** to allow your public IP.

2. **Update the **``** Connection String** Open `appsettings.json` and replace the connection string:

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=AuctionsDB;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
   }
   ```

3. **Apply Migrations to Azure SQL**

   ```sh
   dotnet ef database update
   ```

### **Option 2: Use Local SQL Server**

If you prefer running the database locally:

1. Install **SQL Server Express** or **SQL Server Developer Edition**.
2. Update `appsettings.json`:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=AuctionsDB;Trusted_Connection=True;"
   }
   ```
3. Apply migrations:
   ```sh
   dotnet ef database update
   ```

---
## 🛠 Azure Blob Storage Configuration

### **1. Create an Azure Storage Account**
1. Sign in to Azure Portal.
2. Create a new **Storage Account**.
3. Under **Containers**, create a new container (e.g., `auction-images`).

### **2. Update the Connection String**
1. Open `appsettings.json` and replace the placeholder with your actual Azure Storage connection string:
```json
"AzureStorage": {
    "ConnectionString": "YOUR_AZURE_STORAGE_CONNECTION_STRING"
}
```
2. If you don’t have the connection string and want to use the existing Listings' images, contact the project maintainer.

### **3. Upload Images to Blob Storage**
- When creating a listing, images will be uploaded to the Azure Blob Storage container.
- The application will generate a public URL for each image and store it in the database.

## 🏃‍♂️ Running the Application

To run the application, execute the following command:

```sh
 dotnet run
```

Then, open your browser and navigate to:

```sh
 https://localhost:5001
```

If you deployed to **Azure App Service**, use the provided **public URL**.

---

## 🔧 Common Issues & Fixes

### **1. "The server was not found or was not accessible"**

- Ensure your **Azure SQL firewall** allows your **current IP**.
- Check if your **connection string** is correct.

### **2. **``** Not Found**

If you see an error while running migrations:

```sh
 dotnet tool install --global dotnet-ef
```

### **3. Images Not Displaying**

- Ensure images are uploaded to **wwwroot/Images/**.
- Verify the `<img>` tag in `Index.cshtml` uses the correct path.

---

## 🛠 Deployment to Azure (Optional)

1. **Right-click the project in Visual Studio** → **Publish**.
2. Select **Azure App Service** and deploy.
3. Visit your **Azure-provided URL**.

---

## ©️ Credits

- ### **Instructor**: Nuhi Besimi & Aleksandar Petkovski
- ### **Teammates**: Lavdrim Mustafi, Alban Zulfija & Nejazi Shabani

---

## 🤝 Contributing

Feel free to submit **pull requests** or report **issues**.

---

## 📞 Contact

For any inquiries, reach out at [**lavdrim.mustafi03@gmail.com**](mailto\:your-email@example.com) or [**lm30969@seeu.edu.mk**](mailto\:your-email@example.com).

---

### 🎉 Now you're ready to run and deploy your AuctionsApp! 🚀

