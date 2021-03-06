﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Robops.Lib.Camara.Leg.Gabinete
{
    public class PessoalModel
    {
        [Key]
        public string Key
        {
            get {
                return $"{IdDeputado}-{Ano}-{NomeFuncionario}";
            }
            set { /* ignore */ }
        }

        public int IdDeputado { get; set; }
        public string NomeDeputado { get; set; }
        public int Ano { get; set; }
        public string NomeFuncionario { get; set; }
        public string GrupoFuncional { get; set; }
        public string Cargo { get; set; }
        public DateTime InicioExercicio { get; set; }
        public DateTime FimExercicio { get; set; }
        public string UrlRemuneracao { get; set; }

    }
}

