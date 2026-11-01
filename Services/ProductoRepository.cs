using Microsoft.Data.SqlClient;
using ProductosApi.Models;

namespace ProductosApi.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly string _cs;

    public ProductoRepository(IConfiguration config)
    {
        _cs = config.GetConnectionString("Default")!;
    }

    private static Producto Mapear(SqlDataReader r) => new()
    {
        Id = r.GetInt32(0),
        Nombre = r.GetString(1),
        Descripcion = r.IsDBNull(2) ? null : r.GetString(2),
        Precio = r.GetDecimal(3),
        Stock = r.GetInt32(4)
    };

    public List<Producto> ObtenerTodos()
    {
        var lista = new List<Producto>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("SELECT Id, Nombre, Descripcion, Precio, Stock FROM Productos", con);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public Producto? ObtenerPorId(int id)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("SELECT Id, Nombre, Descripcion, Precio, Stock FROM Productos WHERE Id = @id", con);
        cmd.Parameters.AddWithValue("@id", id);
        con.Open();
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public int Insertar(Producto p)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            @"INSERT INTO Productos (Nombre, Descripcion, Precio, Stock)
              OUTPUT INSERTED.Id
              VALUES (@nombre, @desc, @precio, @stock)", con);
        cmd.Parameters.AddWithValue("@nombre", p.Nombre);
        cmd.Parameters.AddWithValue("@desc", (object?)p.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@precio", p.Precio);
        cmd.Parameters.AddWithValue("@stock", p.Stock);
        con.Open();
        return (int)cmd.ExecuteScalar()!;
    }

    public bool Actualizar(Producto p)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            @"UPDATE Productos
              SET Nombre = @nombre, Descripcion = @desc, Precio = @precio, Stock = @stock
              WHERE Id = @id", con);
        cmd.Parameters.AddWithValue("@id", p.Id);
        cmd.Parameters.AddWithValue("@nombre", p.Nombre);
        cmd.Parameters.AddWithValue("@desc", (object?)p.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@precio", p.Precio);
        cmd.Parameters.AddWithValue("@stock", p.Stock);
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("DELETE FROM Productos WHERE Id = @id", con);
        cmd.Parameters.AddWithValue("@id", id);
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public List<Producto> Buscar(string nombre)
    {
        var lista = new List<Producto>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "SELECT Id, Nombre, Descripcion, Precio, Stock FROM Productos WHERE Nombre LIKE @nombre", con);
        cmd.Parameters.AddWithValue("@nombre", $"%{nombre}%");
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }
}
