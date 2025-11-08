using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Options
{
    /// <summary>
    /// Hourly Weather Variables (https://open-meteo.com/en/docs)
    /// </summary>
    public class HourlyOptions : IEnumerable<HourlyOptionsParameter>, ICollection<HourlyOptionsParameter>
    {
        public static HourlyOptions All { get { return new HourlyOptions((HourlyOptionsParameter[])Enum.GetValues(typeof(HourlyOptionsParameter))); } }

        /// <summary>
        /// A copy of the current applied parameter. This is a COPY. Editing anything inside this copy won't be applied 
        /// </summary>
        public List<HourlyOptionsParameter> Parameter { get { return new List<HourlyOptionsParameter>(_parameter); } }

        public int Count => _parameter.Count;

        public bool IsReadOnly => false;

        private readonly List<HourlyOptionsParameter> _parameter;

        public HourlyOptions(HourlyOptionsParameter parameter)
        {
            _parameter = new List<HourlyOptionsParameter>();
            Add(parameter);
        }

        public HourlyOptions(HourlyOptionsParameter[] parameter)
        {
            _parameter = new List<HourlyOptionsParameter>();
            Add(parameter);
        }

        public HourlyOptions()
        {
            _parameter = new List<HourlyOptionsParameter>();
        }

        public HourlyOptionsParameter this[int index]
        {
            get { return _parameter[index]; }
            set
            {
                _parameter[index] = value;
            }
        }

        public void Add(HourlyOptionsParameter param)
        {
            // Check that the parameter isn't already added
            if (_parameter.Contains(param)) return;

            _parameter.Add(param);
        }

        public void Add(HourlyOptionsParameter[] param)
        {
            foreach (HourlyOptionsParameter paramToAdd in param)
            {
                Add(paramToAdd);
            }
        }

        public IEnumerator<HourlyOptionsParameter> GetEnumerator()
        {
            return _parameter.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _parameter.Clear();
        }

        public bool Contains(HourlyOptionsParameter item)
        {
            return _parameter.Contains(item);
        }

        public void CopyTo(HourlyOptionsParameter[] array, int arrayIndex)
        {
            _parameter.CopyTo(array, arrayIndex);
        }

        public bool Remove(HourlyOptionsParameter item)
        {
            return _parameter.Remove(item);
        }

    }

    // This is converted to string so it has to be the exact same name like in 
    // https://open-meteo.com/en/docs #Hourly Parameter Definition
    public enum HourlyOptionsParameter
    {
        temperature_2m,
        relativehumidity_2m,
        dewpoint_2m,
        apparent_temperature,
        precipitation,
        rain,
        showers,
        snowfall,
        snow_depth,
        snowfall_height,
        freezing_level_height,
        weathercode,
        pressure_msl,
        surface_pressure,
        cloudcover,
        cloudcover_low,
        cloudcover_mid,
        cloudcover_high,
        visibility,
        evapotranspiration,
        et0_fao_evapotranspiration,
        vapor_pressure_deficit,
        cape,
        windspeed_10m,
        windspeed_80m,
        windspeed_120m,
        windspeed_180m,
        winddirection_10m,
        winddirection_80m,
        winddirection_120m,
        winddirection_180m,
        windgusts_10m,
        temperature_80m,
        temperature_120m,
        temperature_180m,
        soil_temperature_0cm,
        soil_temperature_6cm,
        soil_temperature_18cm,
        soil_temperature_54cm,
        soil_moisture_0_1cm,
        soil_moisture_1_3cm,
        soil_moisture_3_9cm,
        soil_moisture_9_27cm,
        soil_moisture_27_81cm,
        shortwave_radiation,
        direct_radiation,
        diffuse_radiation,
        direct_normal_irradiance,
        terrestrial_radiation,
        shortwave_radiation_instant,
        direct_radiation_instant,
        diffuse_radiation_instant,
        direct_normal_irradiance_instant,
        terrestrial_radiation_instant,
        temperature_1000hPa,
        temperature_975hPa,
        temperature_950hPa,
        temperature_925hPa,
        temperature_900hPa,
        temperature_875hPa,
        temperature_850hPa,
        temperature_825hPa,
        temperature_800hPa,
        temperature_775hPa,
        temperature_750hPa,
        temperature_725hPa,
        temperature_700hPa,
        temperature_675hPa,
        temperature_650hPa,
        temperature_625hPa,
        temperature_600hPa,
        temperature_575hPa,
        temperature_550hPa,
        temperature_525hPa,
        temperature_500hPa,
        temperature_475hPa,
        temperature_450hPa,
        temperature_425hPa,
        temperature_400hPa,
        temperature_300hPa,
        temperature_250hPa,
        temperature_200hPa,
        temperature_150hPa,
        temperature_100hPa,
        temperature_70hPa,
        temperature_50hPa,
        temperature_30hPa,
        dewpoint_1000hPa,
        dewpoint_975hPa,
        dewpoint_950hPa,
        dewpoint_925hPa,
        dewpoint_900hPa,
        dewpoint_875hPa,
        dewpoint_850hPa,
        dewpoint_825hPa,
        dewpoint_800hPa,
        dewpoint_775hPa,
        dewpoint_750hPa,
        dewpoint_725hPa,
        dewpoint_700hPa,
        dewpoint_675hPa,
        dewpoint_650hPa,
        dewpoint_625hPa,
        dewpoint_600hPa,
        dewpoint_575hPa,
        dewpoint_550hPa,
        dewpoint_525hPa,
        dewpoint_500hPa,
        dewpoint_475hPa,
        dewpoint_450hPa,
        dewpoint_425hPa,
        dewpoint_400hPa,
        dewpoint_300hPa,
        dewpoint_250hPa,
        dewpoint_200hPa,
        dewpoint_150hPa,
        dewpoint_100hPa,
        dewpoint_70hPa,
        dewpoint_50hPa,
        dewpoint_30hPa,
        relativehumidity_1000hPa,
        relativehumidity_975hPa,
        relativehumidity_950hPa,
        relativehumidity_925hPa,
        relativehumidity_900hPa,
        relativehumidity_875hPa,
        relativehumidity_850hPa,
        relativehumidity_825hPa,
        relativehumidity_800hPa,
        relativehumidity_775hPa,
        relativehumidity_750hPa,
        relativehumidity_725hPa,
        relativehumidity_700hPa,
        relativehumidity_675hPa,
        relativehumidity_650hPa,
        relativehumidity_625hPa,
        relativehumidity_600hPa,
        relativehumidity_575hPa,
        relativehumidity_550hPa,
        relativehumidity_525hPa,
        relativehumidity_500hPa,
        relativehumidity_475hPa,
        relativehumidity_450hPa,
        relativehumidity_425hPa,
        relativehumidity_400hPa,
        relativehumidity_300hPa,
        relativehumidity_250hPa,
        relativehumidity_200hPa,
        relativehumidity_150hPa,
        relativehumidity_100hPa,
        relativehumidity_70hPa,
        relativehumidity_50hPa,
        relativehumidity_30hPa,
        cloudcover_1000hPa,
        cloudcover_975hPa,
        cloudcover_950hPa,
        cloudcover_925hPa,
        cloudcover_900hPa,
        cloudcover_875hPa,
        cloudcover_850hPa,
        cloudcover_825hPa,
        cloudcover_800hPa,
        cloudcover_775hPa,
        cloudcover_750hPa,
        cloudcover_725hPa,
        cloudcover_700hPa,
        cloudcover_675hPa,
        cloudcover_650hPa,
        cloudcover_625hPa,
        cloudcover_600hPa,
        cloudcover_575hPa,
        cloudcover_550hPa,
        cloudcover_525hPa,
        cloudcover_500hPa,
        cloudcover_475hPa,
        cloudcover_450hPa,
        cloudcover_425hPa,
        cloudcover_400hPa,
        cloudcover_300hPa,
        cloudcover_250hPa,
        cloudcover_200hPa,
        cloudcover_150hPa,
        cloudcover_100hPa,
        cloudcover_70hPa,
        cloudcover_50hPa,
        cloudcover_30hPa,
        windspeed_1000hPa,
        windspeed_975hPa,
        windspeed_950hPa,
        windspeed_925hPa,
        windspeed_900hPa,
        windspeed_875hPa,
        windspeed_850hPa,
        windspeed_825hPa,
        windspeed_800hPa,
        windspeed_775hPa,
        windspeed_750hPa,
        windspeed_725hPa,
        windspeed_700hPa,
        windspeed_675hPa,
        windspeed_650hPa,
        windspeed_625hPa,
        windspeed_600hPa,
        windspeed_575hPa,
        windspeed_550hPa,
        windspeed_525hPa,
        windspeed_500hPa,
        windspeed_475hPa,
        windspeed_450hPa,
        windspeed_425hPa,
        windspeed_400hPa,
        windspeed_300hPa,
        windspeed_250hPa,
        windspeed_200hPa,
        windspeed_150hPa,
        windspeed_100hPa,
        windspeed_70hPa,
        windspeed_50hPa,
        windspeed_30hPa,
        winddirection_1000hPa,
        winddirection_975hPa,
        winddirection_950hPa,
        winddirection_925hPa,
        winddirection_900hPa,
        winddirection_875hPa,
        winddirection_850hPa,
        winddirection_825hPa,
        winddirection_800hPa,
        winddirection_775hPa,
        winddirection_750hPa,
        winddirection_725hPa,
        winddirection_700hPa,
        winddirection_675hPa,
        winddirection_650hPa,
        winddirection_625hPa,
        winddirection_600hPa,
        winddirection_575hPa,
        winddirection_550hPa,
        winddirection_525hPa,
        winddirection_500hPa,
        winddirection_475hPa,
        winddirection_450hPa,
        winddirection_425hPa,
        winddirection_400hPa,
        winddirection_300hPa,
        winddirection_250hPa,
        winddirection_200hPa,
        winddirection_150hPa,
        winddirection_100hPa,
        winddirection_70hPa,
        winddirection_50hPa,
        winddirection_30hPa,
        geopotential_height_1000hPa,
        geopotential_height_975hPa,
        geopotential_height_950hPa,
        geopotential_height_925hPa,
        geopotential_height_900hPa,
        geopotential_height_875hPa,
        geopotential_height_850hPa,
        geopotential_height_825hPa,
        geopotential_height_800hPa,
        geopotential_height_775hPa,
        geopotential_height_750hPa,
        geopotential_height_725hPa,
        geopotential_height_700hPa,
        geopotential_height_675hPa,
        geopotential_height_650hPa,
        geopotential_height_625hPa,
        geopotential_height_600hPa,
        geopotential_height_575hPa,
        geopotential_height_550hPa,
        geopotential_height_525hPa,
        geopotential_height_500hPa,
        geopotential_height_475hPa,
        geopotential_height_450hPa,
        geopotential_height_425hPa,
        geopotential_height_400hPa,
        geopotential_height_300hPa,
        geopotential_height_250hPa,
        geopotential_height_200hPa,
        geopotential_height_150hPa,
        geopotential_height_100hPa,
        geopotential_height_70hPa,
        geopotential_height_50hPa,
        geopotential_height_30hPa,
        is_day,
        uv_index,
        uv_index_clear_sky,
        precipitation_probability

    }
}