﻿using altsite.Controllers;
using altsite.Models;
using MongoDB.Driver;

namespace altsite.MongoControllers;

public class SiteMongoController : IMongoController<Site>
{
    private readonly IMongoCollection<Site> siteCollection;
    

    public SiteMongoController(IMongoCollection<Site> _siteCollection)
    {
        siteCollection = _siteCollection;
    }
    
    public async Task<Site?> Get(string id)
    {
        var builder = Builders<Site>.Filter;
        var filter = builder.Eq(site => NormalizeDomain(site.OriginalSiteURL), id);
        return await siteCollection.Find(filter).FirstOrDefaultAsync();
    }
    private string NormalizeDomain(string host)
    {
        var parts = host.Split('.');
        if (parts.Length > 2)
        {
            //remove the subdomain
            return string.Join(".", parts.Skip(1));
        }
        return host;
    }

    public async Task<List<Site>> GetCollection()
    {
        return await siteCollection.Find(Builders<Site>.Filter.Empty).ToListAsync();
    }

    public async Task Set(Site value)
    {
        await siteCollection.InsertOneAsync(value);
    }

    public async Task Update(Site value)
    {
        var builder = Builders<Site>.Filter;
        var filter = builder.Eq(site => site._id, value._id);

        await siteCollection.ReplaceOneAsync(filter,value);
    }

    public async Task Remove(Site value)
    {
        var builder = Builders<Site>.Filter;
        var filter = builder.Eq(site => site._id, value._id);
        await siteCollection.DeleteOneAsync(filter);
    }
}