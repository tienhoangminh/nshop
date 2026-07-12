// Global using directives

global using System.Text.Json;
global using AutoMapper;
global using Elastic.Clients.Elasticsearch;
global using MediatR;
global using MediatR.NotificationPublishers;
global using Microsoft.Extensions.Caching.Distributed;
global using nShop.AppDefaults;
global using nShop.Catalog.Client.Abstractions.Dtos;
global using nShop.Catalog.DomainEvents;
global using nShop.Catalog.Elasticsearch;
global using nShop.Catalog.IntegrationEvents;
global using nShop.Catalog.SyncService;
global using nShop.Catalog.SyncService.Abstractions;
global using nShop.Catalog.SyncService.MongoDb;
global using nShop.Infrastructure.KafkaPullService;
global using nShop.Shared;