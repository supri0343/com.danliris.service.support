﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.danliris.support.lib.ViewModel.Ceisa;
using Newtonsoft.Json;
using com.danliris.support.lib.Interfaces.Ceisa;
using System;
using com.danliris.support.lib.Helpers;

namespace com.danliris.support.lib.Services.Ceisa
{
    public class CeisaService : ICeisaService
    {
        public CeisaService()
        {
    
        }
        public async Task<List<RateValutaViewModel>> GetValutaRate(string kode, string token)
        {

            using (var client = new HttpClient())
            {
                var authCeisa = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authCeisa;

                var response = client.GetAsync($"{APIEndpoint.HostToHost}kurs/{kode}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    List<RateValutaViewModel> viewModel = JsonConvert.DeserializeObject<List<RateValutaViewModel>>(result.GetValueOrDefault("data").ToString()); ;
                    return viewModel;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<LartasViewModel>> GetLartas(string kode, string token)
        {

            using (var client = new HttpClient())
            {
                var authCeisa = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authCeisa;
                var response = client.GetAsync($"{APIEndpoint.HostToHost}hs-lartas?kodeHs={kode}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    List<LartasViewModel> viewModel = JsonConvert.DeserializeObject<List<LartasViewModel>>(result.GetValueOrDefault("data").ToString()); ;
                    return viewModel;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<LartasViewModel>> GetTarifHS(string kode, string token)
        {

            using (var client = new HttpClient())
            {
                var dateNow = DateTime.Now.ToString("yyyy-MM-dd");
                var authCeisa = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authCeisa;
                var response = client.GetAsync($"{APIEndpoint.HostToHost}tarif-hs?kodeHs={kode}&tanggal={dateNow}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    List<LartasViewModel> viewModel = JsonConvert.DeserializeObject<List<LartasViewModel>>(result.GetValueOrDefault("posTarif").ToString()); ;
                    return viewModel;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<ManifesBC11ViewModel> GetManifestBC11(string kodeKantor,string noHostBl,DateTime tglHostBl, string token)
        {

            using (var client = new HttpClient())
            {
                var tglManifes = tglHostBl.ToString("dd-MM-yyyy");
                var authCeisa = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authCeisa;
                var response = client.GetAsync($"{APIEndpoint.HostToHost}manifes-bc11?kodeKantor={kodeKantor}&noHostBl={noHostBl}&tglHostBl={tglManifes}&nama={""}").Result;
                if (response.IsSuccessStatusCode)
                {
                    //var content = response.Content.ReadAsStringAsync().Result;
                    ManifesBC11ViewModel viewModel = JsonConvert.DeserializeObject<ManifesBC11ViewModel>(response.Content.ReadAsStringAsync().Result);
                    //Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    //ManifesBC11ViewModel viewModel = JsonConvert.DeserializeObject<ManifesBC11ViewModel>(result.GetValueOrDefault("data").ToString()); ;
                    return viewModel;
                }
                else
                {
                    throw new Exception("err");
                }
            }
        }


        public class ResponViewModel
        {
            public string nomorAju { get; set; }
            public string nomorDaftar { get; set; }
            public DateTime? tanggalDaftar { get; set; }
            public string kodeProses { get; set; }
            public DateTime? waktuStatus { get; set; }
            public string keterangan { get; set; }
            public string kodeDokumen { get; set; }
        
        }
        public async Task<List<ResponViewModel>> GetRespon(string kode, string token)
        {

            using (var client = new HttpClient())
            {
                var authCeisa = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authCeisa;
                var response = client.GetAsync($"{APIEndpoint.HostToHost}status/{kode}").Result;
                //var response = client.GetAsync($"https://nlehub-dev.kemenkeu.go.id/openapi/status/{kode}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    List<ResponViewModel> viewModel = JsonConvert.DeserializeObject<List<ResponViewModel>>(result.GetValueOrDefault("dataRespon").ToString()); ;
                    return viewModel;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
