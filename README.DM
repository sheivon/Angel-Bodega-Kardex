
Aplicar actualizacion a los tablas del base para que pueda hacer un eliminacion sin eliminar.

`
alter table usuarios add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table categoria add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table area_trabajo add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table proyecto add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table periodo add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table proveedor add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table retira add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table salida add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table autoriza add Eliminado TINYINT(1) NOT NULL DEFAULT 0;
alter table orden_compra add Eliminado TINYINT(1) NOT NULL DEFAULT 0;

`
