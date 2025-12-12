using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PAWProject.Data.Models;
using PAWProject.Data.Repositories;

namespace PAWProject.Core
{
    public interface ISourceService
    {
        Task<IEnumerable<Source>> GetArticlesFromDBAsync(int? id);
        Task<Source?> GetByIdAsync(int id);
        Task<Source> CreateSourceAsync(Source source);
    }

    public class SourceService : ISourceService
    {
        private readonly IRepositorySource _repositorySource;

        public SourceService(IRepositorySource repositorySource)
        {
            _repositorySource = repositorySource ?? throw new ArgumentNullException(nameof(repositorySource));
        }

        public async Task<IEnumerable<Source>> GetArticlesFromDBAsync(int? id)
        {
            if (id == null)
            {
                return await _repositorySource.ReadAsync();
            }

            var found = await _repositorySource.FindAsync(id.Value);
            return found == null ? new List<Source>() : new List<Source> { found };
        }

        public async Task<Source?> GetByIdAsync(int id)
        {
            return await _repositorySource.FindAsync(id);
        }

        public async Task<Source> CreateSourceAsync(Source source)
        {
            // Buscamos métodos candidatos en el repositorio
            var repoType = _repositorySource.GetType();
            var candidateNames = new[] { "CreateAsync", "AddAsync", "InsertAsync", "SaveAsync", "Create", "Add", "Insert" };

            MethodInfo? method = null;
            foreach (var name in candidateNames)
            {
                method = repoType.GetMethod(name, new[] { typeof(Source) });
                if (method != null) break;
            }

            if (method == null)
            {
                // Intentar métodos sin parámetros que hagan persistencia no es seguro; fallamos explícitamente
                throw new NotImplementedException("El repositorio no expone un método Create/Add/Insert que acepte Source. Adapta IRepositorySource o implementa CreateAsync.");
            }

            // Invocar el método
            var result = method.Invoke(_repositorySource, new object[] { source });

            // Si el método devuelve Task<T> o Task, hay que awaitarlo
            if (result is Task taskResult)
            {
                await taskResult.ConfigureAwait(false);

                // Si es Task<Source>
                var resultType = taskResult.GetType();
                if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var returnType = resultType.GetGenericArguments()[0];
                    if (returnType == typeof(Source))
                    {
                        // obtener resultado de Task<Source>
                        var prop = resultType.GetProperty("Result");
                        if (prop != null)
                        {
                            var created = (Source?)prop.GetValue(taskResult);
                            if (created != null) return created;
                        }
                    }
                    if (returnType == typeof(bool))
                    {
                        // Task<bool> -> si true devolvemos la entidad (no tiene Id), si false lanzamos
                        var prop = resultType.GetProperty("Result");
                        var ok = prop != null && (bool)prop.GetValue(taskResult)!;
                        if (!ok) throw new InvalidOperationException("Repositorio devolvió false al crear la fuente.");
                        return source;
                    }
                }

                // Si es Task sin resultado, asumimos que el repo guardó la entidad y la entidad puede tener Id si el repo la actualizó por referencia
                return source;
            }

            // Si el método devuelve directamente Source
            if (result is Source s) return s;

            // Si devuelve bool
            if (result is bool b)
            {
                if (!b) throw new InvalidOperationException("Repositorio devolvió false al crear la fuente.");
                return source;
            }

            throw new NotSupportedException("El método del repositorio devolvió un tipo no soportado. Adapta CreateSourceAsync al método real del repositorio.");
        }
    }
}