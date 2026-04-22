# שלב 1: בנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתקת קובץ הפרויקט - רנדר כבר נמצא בתוך ToDoApi ולכן הקובץ כאן
COPY ToDoApi.csproj .
RUN dotnet restore

# העתקת שאר הקבצים ובנייה
COPY . .
RUN dotnet publish -c Release -o /app/out

# שלב 2: הרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# הגדרת פורט ל-Render
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ToDoApi.dll"]