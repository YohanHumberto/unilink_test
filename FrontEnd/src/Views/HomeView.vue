<template>
    <div>
        <div v-if="usuario.Nombre == ''">
            <div v-if="ViewRegister">
                <RegistroComponent></RegistroComponent>
            </div>
            <div v-else>
                <LoginComponent></LoginComponent>
            </div>
        </div>
        <div v-else class="row p-0 m-0">
            <div class="col-12 col-md-7 m-auto">
                <div class="container" id="container">
                    <div style="margin-top: 50px;">
                        <RuletaComponent />
                    </div>
                    <button id="spin-btn" v-on:click="girar" ref="spinBtn">GIRAR</button>
                    <img src="../assets/spinner-arrow-.svg" alt="spinner-arrow"
                        style="transform: rotate(-90deg); left: 48.4%; top: -12%; " />
                </div>
            </div>
            <div class="col-12 col-md-7 m-auto">
                <HeaderComponent :Nombre="usuario.Nombre" :Monto="usuario.Monto" />
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">VALORES DE APUESTA</h5>
                    </div>
                    <div class="modal-body">
                        <div class="mb-1">
                            <label>Tipo de apuesta:</label>
                            <select v-model="TipoApuesta" v-on:change="onChangeDisabled" class="form-control disabled">
                                <option value="1">Color</option>
                                <option value="2">Color y pares o impares</option>
                                <option value="3">Color y numero</option>
                            </select>
                        </div>
                        <hr />
                        <div class="col-12 row m-0 p-0">
                            <div class="mt-2 col-md-6 p-0">
                                <label>Color: </label>
                                <select v-model="Apuesta.Color" class="form-control">
                                    <option selected>Rojo</option>
                                    <option>Negro</option>
                                </select>
                            </div>
                            <div class="mt-2 col-md-6 p-0 ps-md-2">
                                <label>Tipo de numero: </label>
                                <select v-model="Apuesta.TipoNumero" :disabled="TipoNumeroDisabled"
                                    class="form-control disabled">
                                    <option value="">Ninguno</option>
                                    <option>Par</option>
                                    <option>Inpar</option>
                                </select>
                            </div>
                            <div class="mt-2 col-md-6 p-0">
                                <label>Numero: </label>
                                <input v-model="Apuesta.Numero" min="0" max="36" :disabled="NumeroDisabled" type="number"
                                    class="form-control" />
                            </div>
                            <div class="mt-2 col-md-6 p-0 ps-md-2">
                                <label>Monto: </label>
                                <input v-model="Apuesta.Monto" type="number" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button class="btn btn-success" v-on:click="onApuesta">Iniciar Apuesta</button>
                        <button class="btn btn-primary me-2 ms-2" v-on:click="guardarPartida">Guardar Partida</button>
                    </div>
                </div>
                <div style="display:none;">
                    <ResultadoComponent :Apuesta="Apuesta" :Elemento="Elemento" />
                </div>
            </div>
        </div>
    </div>
</template>
  
<script>

import LoginComponent from "../components/LoginComponent.vue";
import RegistroComponent from "../components/RegistroComponent.vue";
import RuletaComponent from "../components/RuletaComponent.vue";
import ResultadoComponent from "../components/ResultadoComponent.vue";
import HeaderComponent from "../components/HeaderComponent.vue";
import Elements from '../assets/data.js';

export default {
    name: 'HomeView',
    components: {
        LoginComponent,
        RegistroComponent,
        RuletaComponent,
        ResultadoComponent,
        HeaderComponent
    },
    data: () => {
        return {
            usuario: { Nombre: "", Monto: 0 },
            Elemento: { label: null, nameColor: null, tipo: "" },
            ViewRegister: true,
            Apuesta: {
                TipoNumero: "Par",
                Color: "Rojo",
                Numero: "",
                Monto: 0,
                MontoResultante: 0
            },
            TipoNumeroDisabled: true,
            NumeroDisabled: true,
            TipoApuesta: 1,
        }
    },
    methods: {
        showAlert() {

        },
        async girar() {
            let URI = process.env.VUE_APP_RUTA_API;
            const { Number, Color } = await fetch(URI + "/api/GetRandomNumberAndColor").then(response => response.json()).then(({ Data }) => Data);

            this.$refs.spinBtn.disabled = true;
            const referens = this.$children[0].$el;
            const item = Elements.find(a => a.label == Number);
            let degree = (item.minDegree + item.maxDegree) / 2;

            //SETEAMOS EL STYLE
            let vueltas = Math.abs(referens.style.transform.replace("rotate(", "").replace("deg)", "")) < 2160 ? 3600 : 1800;
            referens.style.transform = `rotate(-${degree + vueltas}deg)`;

            setTimeout(() => {
                this.Elemento = { label: Number, nameColor: Color, tipo: Number % 2 == 0 ? "SI" : "NO" };
                this.$refs.spinBtn.disabled = false;
                return true;
            }, 5000)
        },
        async onApuesta() {

            if (this.Apuesta.Monto > this.usuario.Monto) return alert("No tiene suficiente saldo para relizar esta apuesta");
            if (this.Apuesta.Monto <= 0) return alert("La monto minimo para realizar una apuesta es 1$");
            if (!this.NumeroDisabled) {
                if (this.Apuesta.Numero < 0 || this.Apuesta.Numero > 36 || this.Apuesta.Numero == "") return alert("El numero de la apuesta esta fuera de rango");
            }

            await this.girar();

            setTimeout(() => {

                const { TipoNumero, Color, Numero, Monto } = this.Apuesta;
                let URI = process.env.VUE_APP_RUTA_API;

                fetch(URI + `/api/GetMontoApuesta?TipoApuesta=${this.TipoApuesta}&Monto=${Monto}&Color=${Color}&TipoNumber=${TipoNumero}&Numero=${Numero}&RandomNumber=${this.Elemento.label}`)
                    .then(response => response.json())
                    .then(({ Data }) => {

                        this.Apuesta.MontoResultante = Data.montoGanado > 0
                            ? Data.montoGanado
                            : - this.Apuesta.Monto;

                        setTimeout(() => {

                            let html = document.getElementById("resultados").outerHTML;

                            if (Data.montoGanado > 0) {
                                this.$swal({ title: "APUESTA GANADA", html, icon: "success" });
                                return;
                            }

                            this.$swal({ title: "APUESTA PERDIDA", html, icon: "error" });

                            this.usuario.Monto = Data.montoGanado > 0
                                ? this.usuario.Monto + Data.montoGanado
                                : this.usuario.Monto - this.Apuesta.Monto;

                        }, 100)

                    });

            }, 5000);

        },
        onChangeDisabled() {
            switch (parseInt(this.TipoApuesta)) {
                case 1:
                    this.NumeroDisabled = true;
                    this.TipoNumeroDisabled = true;
                    break;
                case 2:
                    this.NumeroDisabled = true;
                    this.TipoNumeroDisabled = false;
                    break;
                case 3:
                    this.NumeroDisabled = false;
                    this.TipoNumeroDisabled = true;
                    break;
            }
        },
        guardarPartida() {
            let URI = process.env.VUE_APP_RUTA_API;

            fetch(URI + "/api/AddUser", {
                method: "POST",
                body: JSON.stringify({ Name: this.usuario.Nombre, Monto: this.usuario.Monto }),
                headers: { 'Content-Type': 'application/json' }
            }).then(response => response.json())
                .then(({ Data }) => {

                    if (!Data.state) {
                        this.$swal("ERROR", Data?.errorMessage, "error");
                        return;
                    }

                    this.$swal({
                        title: "PARTIDA GUARDADA",
                        content: "Su partida ha sido guardada correctamente",
                        icon: "success"
                    });

                });
        }
    }
}
</script>