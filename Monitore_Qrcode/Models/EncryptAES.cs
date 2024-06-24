
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Monitore_Qrcode.Models
{
    public class EncryptAES
    {
public static string EncryptString(string plainText1,string plainText2, string key)
    {
            if (plainText1.Contains('-'))
            {
                plainText1 = plainText1.Replace("-", "__");
            }
            if (plainText2.Contains('-'))
            {
                plainText2 = plainText2.Replace("-", "__");
            }


            byte[] iv = new byte[16]; // Initialize IV with zeros
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText1+"|"+plainText2);
                    }
                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }
       
    }
}

/*
@* @{
    ViewData["Title"] = "Privacy Policy";
}
@model List<Transaction>;

<link rel="stylesheet" href="~/css/data.css" asp-append-version="true" />

<div class="header">
    <nav class="navbar navbar-expand-lg navbar-dark bg fixed-top">
        <div class="container-fluid">
            <button class="navbar-toggler" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasNavbar" aria-controls="offcanvasNavbar">
                <span class="navbar-toggler-icon"></span>
            </button>


            <svg height="60" width="100">
                <defs>
                    <linearGradient id="grad1">
                        <stop offset="0%" stop-color="yellow" />
                        <stop offset="100%" stop-color="red" />
                    </linearGradient>
                    <linearGradient id="grad2">
                        <stop offset="0%" stop-color="orange" />
                        <stop offset="100%" stop-color="red" />
                    </linearGradient>
                </defs>
                <ellipse cx="50" cy="30" rx="50" ry="30" fill="url(#grad2)" />

                <text fill="#ffffff" font-size="15" font-family="Verdana" x="25" y="35">GT-CO</text>
                Sorry, your browser does not support inline SVG.
            </svg>

            <div class="offcanvas offcanvas-start" tabindex="-1" id="offcanvasNavbar" aria-labelledby="offcanvasNavbarLabel">
                <div class="offcanvas-header">
                    <h5 class="offcanvas-title" id="offcanvasNavbarLabel">
                        <svg height="60" width="100">

                            <ellipse cx="50" cy="30" rx="50" ry="30" fill="url(#grad2)" />

                            <text fill="#ffffff" font-size="15" font-family="Verdana" x="25" y="35">GT-CO</text>
                            Sorry, your browser does not support inline SVG.
                        </svg>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                </div>
                <div class="offcanvas-body">
                    <ul class="navbar-nav justify-content-end flex-grow-1 pe-3">
                        <li class="nav-item"><a class=" custom-link" asp-area="" asp-controller="Home" asp-action="Index">Accueil</a> </li>
                        <li class="nav-item"><a class=" custom-link" asp-area="" asp-controller="Home" asp-action="Index">Deconnecter</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </nav>
</div>



<div class="table-wrapper" style="width:100%">
    <h1>Platform to monitor GTplace transaction</h1>

    <div class="table-responsive" style="overflow-x: auto;">


<table id="example" class="display nowrap table table-striped table-bordered  row-border hover responsive" style="width:100%">
        <thead>
            <tr>
                <th>ID</th>
                <th>Nom</th>
               @*  <th>Email</th> *@
                < th > Tel </ th >
               @*  <th>GuId</th> *@
                < th > UserIdCustomer </ th >
                < th > TransactionType </ th >
                < th > AccountToCredit </ th >
                < th > Amount </ th >
                < th > Remarks </ th >
                    < th > Service </ th >
               @*  <th>IsGTBAccount</th>
                <th>IsOrangeMoney</th>
                <th>IsMoovMoney</th>
                <th>IsMTNMoney</th> *@
                < th > StatusQrCode </ th >
                < th > DateTransaction </ th >
                < th > Imprimer </ th >
            </ tr >
        </ thead >
        < tbody >
            @foreach(var transaction in Model)
            {
                    < tr >
                        < td class= "text-center align-middle" > @transaction.Id </ td >
                        < td class= "text-center align-middle" > @transaction.Nom </ td >
                        @* <td class="text-center align-middle">@transaction.Email</td> *@
                        < td class= "text-center align-middle" > @transaction.Tel </ td >
                        @* <td class="text-center align-middle">@transaction.GuId</td> *@
                        < td class= "text-center align-middle" > @transaction.UserIdCustomer </ td >
                        < td class= "text-center align-middle" > @transaction.TransactionType </ td >
                        < td class= "text-center align-middle" > @transaction.AccountToCredit </ td >
                        < td class= "text-center align-middle" > @transaction.Amount </ td >
                        < td class= "text-center align-middle" > @transaction.Remarks </ td >

                        @if(transaction.IsGTBAccount == "1"){
                            < td class= "text-center align-middle" > GTbank </ td >
                        }
                        @if(transaction.IsMTNMoney == "1"){
                            < td class= "text-center align-middle" > MTNMoney </ td >
                        }
                        @if(transaction.IsOrangeMoney == "1"){
                            < td class= "text-center align-middle" > OrangeMoney </ td >
                        }
                        @if(transaction.IsMoovMoney == "1")
                        {
                            < td class= "text-center align-middle" > MoovMoney </ td >
                        }




                        @* <td class="text-center align-middle">@transaction.IsGTBAccount</td>
                        <td class="text-center align-middle">@transaction.IsOrangeMoney</td>
                        <td class="text-center align-middle">@transaction.IsMoovMoney</td>
                        <td class="text-center align-middle">@transaction.IsMTNMoney</td> *@
                        < td class= "text-center align-middle" > @transaction.StatusQrCode </ td >
                        < td class= "text-center align-middle" > @transaction.InsertDate </ td >
                        < td >
                            < form asp - action = "PrintPDF" asp - controller = "Home" method = "post" >
                                < input type = "hidden" id = "idname" name = "idname" value = "@transaction.Id" />

                                < button type = "submit" class= "btn btn-success" style = "height: 20px; padding: 0px 1px; font-size: 14px;margin-left: 10px;" > Imprimer </ button >
                                @* <button type="submit" class="btn btn-danger">Danger</button>
                                <button type="submit">Imprimer</button> *@
                            </ form >
                        </ td >
            </ tr >
            }
        </ tbody >
    </ table >

    </ div >


</ div > *@*/