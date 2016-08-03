using System.Collections;
using System;

[Serializable]
public class UserData {

	public int id;

    public string nombre,
    email,
    reg_id,
    plataforma,
    fbid,
    token,
    foto;

	public string date_month;
	public string date_day;
	public string date_year;

	public byte[] ImgBytes;
	public string temp_img;
	
	//public Dictionary<string, int> ExercisesMetricas;
	
	public UserData(){
		id = 0;
		reg_id = "";
		plataforma = "";
		nombre = "";
		fbid = "";
		email = "";
		token = "";
        foto = "";

    }

	public void save(){

	}

	public void populateUser(string[] row_){
        //regid", "plataforma", "fbid", "token", "nombre", "email
        id = int.Parse( row_ [0] );
		email = row_ [1];
		nombre = row_ [2];
		fbid = row_ [3];
		token = row_ [4];
	}

	public void format_month(string month_){
		int monthInt_ = 0;
		switch (month_) {
		case "ENERO": monthInt_ = 01; break;
		case "FEBRERO": monthInt_ = 02; break;
		case "MARZO": monthInt_ = 03; break;
		case "ABRIL": monthInt_ = 04; break;
		case "MAYO": monthInt_ = 05; break;
		case "JUNIO": monthInt_ = 06; break;
		case "JULIO": monthInt_ = 07; break;
		case "AGOSTO": monthInt_ = 08; break;
		case "SEPTIEMBRE": monthInt_ = 09; break;
		case "OCTUBRE": monthInt_ = 10; break;
		case "NOVIEMBRE": monthInt_ = 11; break;
		case "DICIEMBRE": monthInt_ = 12; break;
		}
		date_month = monthInt_.ToString();
	}

    public string query_get_artists() {
        //string artists = "select id, artista, descripcion, imagen from contenidos_artistas where id IN "+
        //    "( select contenidos_artistas_id from contenidos where id IN ( select contenidos_id from contenidos_usuarios where usuarios_id = '"+id+"' ) )";

        string artists = "select ca.id, ca.artista " +
                            "from contenidos_artistas ca ";
        return artists;
    }

    public string query_get_contents(string artistas_id) {
        string query = "select id, titulo, contenido_tipo, "+
            " ( select id from contenidos_usuarios where usuarios_id = '"+id+ "' and contenidos_id = contenidos.id ) as is_desbloqueado, fecha_inicio " +
            " from contenidos where contenidos_artistas_id = '" + artistas_id + "' ";

        return query;
    }

    public string query_get_Uartists()
    {
        //string artists = "select id, artista, descripcion, imagen from contenidos_artistas where id IN "+
        //    "( select contenidos_artistas_id from contenidos where id IN ( select contenidos_id from contenidos_usuarios where usuarios_id = '"+id+"' ) )";

        string artists = "select COUNT(c.id) as count_contenidos, ca.id, ca.artista " +
                            "from contenidos c " +
                            "inner join contenidos_artistas ca on ca.id = c.contenidos_artistas_id " +
                            "inner join contenidos_usuarios cu on cu.contenidos_id = c.id " +
                            "where cu.usuarios_id = '" + id + "' " +
                            "group by ca.id ";
        return artists;
    }

    public string query_get_Ucontents(string artistas_id)
    {
        string query = "select id, titulo, contenido_tipo from contenidos where contenidos_artistas_id = '" + artistas_id + "' and "+
            " id IN ( select contenidos_id from contenidos_usuarios where usuarios_id = '"+id+"' ) ";

        return query;
    }

    public string query_get_rand_contenido(string actFecha)
    {
        string query = "select id, titulo, contenido_tipo, contenido, imagen from contenidos  "+
            " where id NOT IN ( select contenidos_id from contenidos_usuarios where usuarios_id = '" + id+ "' ) "+
            " and ( ( tipo_vencimiento = 'fecha' and '"+ actFecha + "' >= fecha_inicio ) or tipo_vencimiento = 'tiempo' ) " +
            " ORDER BY fecha_inicio ASC LIMIT 1 ";
        
        return query;
    }

    public string query_get_rand_contenido_old(string actFecha)
    {
        string query = "select id, titulo, contenido_tipo, contenido, imagen from contenidos  " +
            " where id NOT IN ( select contenidos_id from contenidos_usuarios where usuarios_id = '" + id + "' ) " +
            " and ( ( tipo_vencimiento = 'fecha' and '" + actFecha + "' < fecha_inicio ) or tipo_vencimiento = 'tiempo' ) " +
            " ORDER BY RANDOM() LIMIT 1 ";

        return query;
    }
}
