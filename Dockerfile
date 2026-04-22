FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# העתקה של קובץ הפרויקט מהתיקייה
COPY ToDoApi/ToDoApi.csproj ./ToDoApi/
RUN dotnet restore ./ToDoApi/ToDoApi.csproj

# העתקת כל שאר הקבצים של ה-API
COPY ToDoApi/ ./ToDoApi/
WORKDIR /app/ToDoApi

# בנייה ופרסום
RUN dotnet publish -c Release -o /app/out

# שלב ההרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ToDoApi.dll"]