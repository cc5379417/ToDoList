# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# העתקת קובץ הפרויקט (אנחנו כבר בתוך תיקיית ToDoApi לפי הגדרות Render)
COPY ToDoApi.csproj ./
RUN dotnet restore

# העתקת כל השאר ובנייה
COPY . ./
RUN dotnet publish -c Release -o out

# שלב ההרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# הגדרת פורט
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ToDoApi.dll"]