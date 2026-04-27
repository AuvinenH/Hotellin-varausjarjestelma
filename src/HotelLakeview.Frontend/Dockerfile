FROM node:20-alpine AS build
WORKDIR /app

COPY src/HotelLakeview.Frontend/package*.json ./
RUN npm install

COPY src/HotelLakeview.Frontend/ ./
RUN npm run build

FROM nginx:1.27-alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=build /app/dist ./
COPY src/HotelLakeview.Frontend/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
