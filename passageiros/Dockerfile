FROM golang:1.22-alpine AS build
WORKDIR /src
COPY go.mod go.sum ./
RUN go mod download
COPY . .
RUN CGO_ENABLED=0 go build -o /app .

FROM alpine:3.20
WORKDIR /
COPY --from=build /app /app
EXPOSE 8081
ENTRYPOINT ["/app"]