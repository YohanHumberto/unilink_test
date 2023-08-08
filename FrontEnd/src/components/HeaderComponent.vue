<template>
    <div class="col-12 col-md-7 m-auto mt-2 offset-1">
        <table class="table table-secondary table-bordered border-dark">
            <tr>
                <th scope="col">NOMBRE: {{ Nombre }}</th>
                <th scope="col">MONTO : {{ Monto }} </th>
            </tr>
        </table>
    </div>
</template>
  
<script>
export default {
    name: 'HeaderComponent',
    props: ['Nombre', 'Monto']
}
</script>
