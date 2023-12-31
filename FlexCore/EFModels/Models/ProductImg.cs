﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModels.Models
{
    public partial class ProductImg
    {
        [Key]
        public int ProductImgId { get; set; }
        [Required]
        [StringLength(254)]
        [Unicode(false)]
        public string fk_ProductId { get; set; }
        public int? fk_ColorId { get; set; }
        [Required]
        [StringLength(100)]
        public string ImgPath { get; set; }

        [ForeignKey("fk_ColorId")]
        [InverseProperty("ProductImgs")]
        public virtual ColorCategory fk_Color { get; set; }
        [ForeignKey("fk_ProductId")]
        [InverseProperty("ProductImgs")]
        public virtual Product fk_Product { get; set; }
    }
}