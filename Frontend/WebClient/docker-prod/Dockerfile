FROM node:14-alpine AS build

WORKDIR /usr/local/app

COPY package.json .
COPY package-lock.json .

RUN npm ci

COPY . .

RUN npx ng build

FROM nginx:1.23-alpine

COPY --from=build /usr/local/app/dist/vsp-client /usr/share/nginx/html

COPY docker-prod/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80