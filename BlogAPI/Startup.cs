using BlogAPI.Src.Contextos;
using BlogAPI.Src.Repositorios;
using BlogAPI.Src.Repositorios.Implementacoes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

           
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            // Configura��o de Banco de dados

            services.AddDbContext<BlogPessoalContexto>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Repositorios
            services.AddScoped<IUsuario, UsuarioRepositorio>();
            services.AddScoped<ITema, TemaRepositorio>();
            services.AddControllers();
            services.AddScoped<IPostagem, PostagemRepositorio>();
            // Configura��o Swagger
           // Configura��o do Token Autentica��o JWTBearer
            var  chave  =  Codifica��o . ASCII . GetBytes ( Configura��o [ " Configura��es:Segredo " ]);
            servi�os . AdicionarAutentica��o ( a  =>
                {
                    um . DefaultAuthenticateScheme  =  JwtBearerDefaults . Esquema de autentica��o ;
                    um . DefaultChallengeScheme  =  JwtBearerDefaults . Esquema de autentica��o ;
                }). AddJwtBearer ( b  =>
                {
                    b . RequireHttpsMetadata  =  false ;
                    b . SaveToken  =  true ;
                    b . TokenValidationParameters  =  new  TokenValidationParameters
                    {
                        ValidateIssuerSigningKey  =  true ,
                        IssuerSigningKey  =  new  SymmetricSecurityKey ( chave ),
                        ValidateIssuer  =  false ,
                        ValidateAudience  =  false
                    };
                }
            );

            // Configura��o Swagger
            servi�os . AddSwaggerGen ( s  => {
            s . SwaggerDoc ( " v1 " , new  OpenApiInfo { Title  =  " Blog Pessoal " , Version  =  " v1 " });
            s . AddSecurityDefinition (
                " Portador " ,
                new  OpenApiSecurityScheme ()
                {
                    Nome  =  " Autoriza��o " ,
                    Tipo  =  SecuritySchemeType . ApiKey ,
                    Esquema  =  " Portador " ,
                    BearerFormat  =  " JWT " ,
                    In  =  ParameterLocation . Cabe�alho ,
                    Description  =  " Utiliza��o do cabe�alho de autoriza��o JWT: Portador + Token JWT " ,
                }
                );
                s . AddSecurityRequirement (
                    novo  OpenApiSecurityRequirement
                    {
                        {
                            novo  OpenApiSecurityScheme
                            {
                                Refer�ncia  =  novo  OpenApiReference
                                {
                                    Tipo  =  ReferenceType . SecurityScheme ,
                                    Id  =  " Portador "
                                }
                            },
                            nova  Lista < string >()
                        }
                    }
                );

                var  xmlFile  =  $" { Assembly . GetExecutingAssembly (). GetName (). Name }.xml " ;
                var  xmlPath  =  Caminho . Combine ( AppContext . BaseDirectory , xmlFile );
                s . IncludeXmlComments ( xmlPath );
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BlogPessoalContexto contexto)
        {

            // Ambiente de desenvolvimento
            if (env.IsDevelopment())
            {
                contexto.Database.EnsureCreated();

                app.UseDeveloperExceptionPage();
               contexto.Database.EnsureCreated();
            }
            contexto.Database.EnsureCreated();
            
            // Ambiente de produ��o
            // Rotas
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
