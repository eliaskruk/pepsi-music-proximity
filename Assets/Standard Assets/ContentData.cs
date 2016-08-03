using System.Collections;
using System;

[Serializable]
public class ContentData {
    
    public int id;

    public string titulo,
    subtitulo,
    contenido,
    contenido_tipo,
    contenidos_artistas_id,
    tiempo_activo,
    fecha_inicio,
    fecha_fin,
    imagen,
    tipo_vencimiento,
    precargado,
    serverupdate;

    public string artista , artista_descripcion, artista_imagen;

	public byte[] ImgBytes;
	
	public ContentData(){
		id = 0;
        titulo = "";
        subtitulo = "";
        contenido = "";
        contenido_tipo = "";
        contenidos_artistas_id = "";
        tiempo_activo = "";
        fecha_inicio = "";
        fecha_fin = "";
        imagen = "";
        tipo_vencimiento = "";
        precargado = "";
        serverupdate = "";
        artista_imagen = "";
    }

	public void save(){

	}

	public void populateContent(string[] row_){
        //"id", "titulo", "subtitulo", "contenido", "contenido_tipo", "contenidos_artistas_id", 
        //"tiempo_activo", "fecha_inicio", "fecha_fin", "imagen", "tipo_vencimiento", "serverupdate"
        //ca.artista, ca.descripcion, ca.imagen as artista_imagen

        id = int.Parse( row_ [0] );

        titulo = row_[1];
        subtitulo = row_[2];
        contenido = row_[3];
        contenido_tipo = row_[4];
        contenidos_artistas_id = row_[5];
        tiempo_activo = row_[6];
        fecha_inicio = row_[7];
        fecha_fin = row_[8];
        imagen = row_[9];
        tipo_vencimiento = row_[10];
        precargado = row_[11];
        serverupdate = row_[12];

        artista = row_[13];
        artista_descripcion = row_[14];
        artista_imagen = row_[15];
    }

    public string query_get_contenido(string contenido_id = "") {
        string query = " select c.id, c.titulo, c.subtitulo, c.contenido, c.contenido_tipo, c.contenidos_artistas_id, c.tiempo_activo, " +
            "c.fecha_inicio, c.fecha_fin, c.imagen, c.tipo_vencimiento, c.precargado, c.serverupdate, ca.artista, ca.descripcion, ca.imagen as artista_imagen " +
            "from contenidos c inner join contenidos_artistas ca on ca.id = c.contenidos_artistas_id where c.id = '"+ contenido_id + "' ";
        return query;
    }
}
