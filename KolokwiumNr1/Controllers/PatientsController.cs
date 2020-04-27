using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using KolokwiumNr1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KolokwiumNr1.Controllers
{
    [ApiController]
    [Route("api/medicaments")]
    public class PatientsController : ControllerBase
    {
        private const string ConnString = "Data Source=db-mssql;Initial Catalog=s18371;Integrated Security=True";
        public IConfiguration Configuration { get; set; }
        //[Route("medicaments")]
        [HttpGet("{id}")]
        public IActionResult GetMedicament(string id)
        {
            var listaP = new List<Persctriprion>();
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
            
                com.Connection = con;
                con.Open();
                com.CommandText = "select idmedicament from medicament where idmedicament = @id";
                com.Parameters.AddWithValue("id", id);
                var dr = com.ExecuteReader();
                int jest = 0;
                
                if (dr.Read())
                {
                    jest = (int)dr["IdMedicament"];
                }
                if (jest == 0)
                {
                    return BadRequest("Nie ma takiego leku");
                }
                dr.Close();
                com.CommandText = "select Prescription.IdPrescription, Prescription.Date, Prescription.DueDate, Prescription.IdPatient, Prescription.IdDoctor, Prescription_Medicament.Dose, Prescription_Medicament.Details from Prescription join Prescription_Medicament on Prescription.IdPrescription = Prescription_Medicament.IdPrescription join Medicament on Prescription_Medicament.IdMedicament = Medicament.IdMedicament where Medicament.IdMedicament = @id2 order by date desc";
                com.Parameters.AddWithValue("id2", id);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var listaLekow = new Persctriprion();
                    listaLekow.id = (int)dr["IdPrescription"];
                    listaLekow.date = dr["Date"].ToString();
                    listaLekow.dueDtate = dr["DueDate"].ToString();
                    listaLekow.idPat = (int)dr["idPatient"];
                    listaLekow.idDoc = (int)dr["IdDoctor"];
                    listaLekow.dose = (int)dr["Dose"];
                    listaLekow.details = dr["Details"].ToString();
                    listaP.Add(listaLekow);
                }
                dr.Close();

            }
            return Ok(listaP);
        }

      
        [Route("patients")]
        [HttpDelete("{id}")]
        public IActionResult DeletePatient(string id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                con.Open();
                com.Connection = con;
                SqlTransaction trans = con.BeginTransaction();
                com.Transaction = trans;
           
                com.CommandText = "select * from patient where idPatient = @id";
                com.Parameters.AddWithValue("id", id);
                var dr = com.ExecuteReader();
                int jest = 0;
                if (dr.Read())
                {
                    jest = (int)dr["idPatient"];
                }
                dr.Close();
                if (jest == 0)
                {
                    return BadRequest("nie ma takiego pacjenta");
                }
                else
                {
                    com.CommandText = "select * from Prescription where idPatient = @id";
                    com.Parameters.AddWithValue("id", id);
                    dr = com.ExecuteReader();
                    jest = 0;
                    if (dr.Read())
                    {
                        jest = (int)dr["IdPrescription"];
                    }
                }
                if (jest == 0)
                {
                    try
                    {
                        com.CommandText = "delete from patient where idpatient=@id2";
                        com.Parameters.AddWithValue("id2", id);
                        com.ExecuteNonQuery();
                        trans.Commit();
                    }catch(Exception e)
                    {
                        trans.Rollback();
                        return BadRequest("Wystapil blad");
                    }
                }
                else
                {
                    try
                    {
                        com.CommandText = "delete from Prescription where idPatient=@id3";
                        com.Parameters.AddWithValue("id3", id);
                        com.ExecuteNonQuery();
                        com.CommandText = "delete forom Patient where idPatient=@id4";
                        com.Parameters.AddWithValue("id4", id);
                        com.ExecuteNonQuery();
                        trans.Commit();
                    }catch(Exception e)
                    {
                        trans.Rollback();
                        return BadRequest("Wystapil blad");
                    }
                }

                return Ok("Usunięto pacjenta i jego recepty");
            }
        }
    }
}
