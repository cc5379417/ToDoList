# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתקה של קובץ הפרויקט עם הנתיב המלא מהשורש
COPY ToDoApi/ToDoApi.csproj ToDoApi/
RUN dotnet restore "ToDoApi/ToDoApi.csproj"

# העתקת שאר הקבצים של ה-API
COPY ToDoApi/ ToDoApi/
WORKDIR "/src/ToDoApi"

# בנייה ופרסום
RUN dotnet build "ToDoApi.csproj" -c Release -o /app/build
RUN dotnet publish "ToDoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# שלב ההרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# הגדרת פורט 80 עבור Render
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ToDoApi.dll"]